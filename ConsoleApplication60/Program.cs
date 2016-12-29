using System;
using System.Collections.Generic;

namespace QueueMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            var w = new TestModel();

            var p1 = w.Subscribe();
            var p2 = w.Subscribe();

            p1.Action1();
            p1.Action1();
            p1.Action1();
            p2.Action2();
            p2.Action2();
            p2.Action2();


            w.Dispatch();
            Console.WriteLine("-");
            w.Dispatch();
            Console.WriteLine("-");

            p2.Dispose();

            w.Dispatch();
            Console.WriteLine("-");
            w.Dispatch();
            Console.WriteLine("-");
            w.Dispatch();
            Console.WriteLine("-");
            w.Dispatch();
            Console.WriteLine("-");
            w.Dispatch();
            Console.WriteLine("-");
            w.Dispatch();
            Console.WriteLine("-");


            Console.WriteLine("-end-");

            Console.ReadKey();
        }
    }

    public class TestModel : Worker<TestController>
    {
        public int value1 { get; set; }
        public int value2 { get; set; }
    }

    public class TestController : ServiceBase<TestModel>
    {
        public TestController(TestModel model, Queue2 invoker) : base(model, invoker)
        {
        }

        public void Action1() => Invoker(() =>
        {
            Console.WriteLine("-1-");
        });

        public void Action2() => Invoker(() =>
        {
            Console.WriteLine("-2-");
        });
    }





    //TODO：キューの登録・解除の仕方をシンプルにする







    /// <summary>
    /// サービスベース
    /// </summary>
    /// <typeparam name="MODEL"></typeparam>
    public class ServiceBase<MODEL>
    {
        protected MODEL Model;
        Queue2 invoker;
        public ServiceBase(MODEL model, Queue2 invoker)
        {
            Model = model;
            this.invoker = invoker;
        }

        protected Job Invoker(Action action)
        {
            var job = new Job(action);
            invoker.Enqueue(job);
            return job;
        }

        protected Job Interrupt(Action action)
        {
            var job = new Job(action);
            invoker.Interrupt(job);
            return job;
        }

        public void Dispose()
        {
            ((ISubscribable)Model).Unsubscribe(invoker);
        }
    }

    /// <summary>
    /// 実行処理(主体)
    /// </summary>
    /// <typeparam name="SERVICE"></typeparam>
    public class Worker<SERVICE> : ISubscribable
    {
        /// <summary>
        /// パイプラインデータ
        /// </summary>
        List<Queue2> pipelines = new List<Queue2>();

        /// <summary>
        /// パイプライン登録
        /// </summary>
        /// <returns></returns>
        public SERVICE Subscribe()
        {
            var result = new Queue2();
            pipelines.Add(result);
            return (SERVICE)Activator.CreateInstance(typeof(SERVICE), new object[] { this, result });
        }

        /// <summary>
        /// パイプライン解除
        /// </summary>
        /// <param name="invoker"></param>
        public void Unsubscribe(Queue2 invoker)
        {
            pipelines.Remove(invoker);
        }

        /// <summary>
        /// 並列処理実行
        /// </summary>
        public void Dispatch()
        {
            foreach (var pipeline in pipelines)
            {
                var job = pipeline.Dequeue();
                job?.Dispatch();
            }
        }
    }

    /// <summary>
    /// パイプライン破棄用インターフェイス
    /// </summary>
    public interface ISubscribable
    {
        void Unsubscribe(Queue2 invoker);
    }

    /// <summary>
    /// パイプライン
    /// </summary>
    public class Queue2
    {
        List<Job> queue = new List<Job>();

        Queue<Job> que = new Queue<Job>();

        /// <summary>
        /// キュー数
        /// </summary>
        public int Count
        {
            get
            {
                lock (queue)
                {
                    return queue.Count;
                }
            }
        }

        /// <summary>
        /// 通常キュー登録
        /// </summary>
        /// <param name="job"></param>
        public void Enqueue(Job job)
        {
            lock (queue)
            {
                queue.Add(job);
            }
        }

        /// <summary>
        /// 優先キュー登録
        /// </summary>
        /// <param name="job"></param>
        public void Interrupt(Job job)
        {
            lock (queue)
            {
                queue.Insert(0, job);
            }
        }

        /// <summary>
        /// デキュー
        /// </summary>
        /// <returns></returns>
        public Job Dequeue()
        {
            lock (queue)
            {
                if (0 < queue.Count)
                {
                    var result = queue[0];
                    queue.RemoveAt(0);
                    return result;
                }
                return null;
            }
        }
    }

    /// <summary>
    /// 遅延処理
    /// </summary>
    public class Job
    {
        Action action;
        public Job(Action action)
        {
            this.action = action;
        }

        public void Dispatch()
        {
            action();
        }
    }

}
