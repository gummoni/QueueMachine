using System;
using System.Collections.Generic;
using System.Threading;

namespace QueueMachine
{
    /// <summary>
    /// サービスベース
    /// </summary>
    /// <typeparam name="MODEL"></typeparam>
    public class Service<MODEL> : IDeueueable, IDisposable
    {
        List<IDispatchable> queue = new List<IDispatchable>();
        protected MODEL Model;

        public Service(MODEL model)
        {
            Model = model;
        }

        public virtual void Initialize(object sender)
        {
        }

        public virtual void Dispose()
        {
        }

        protected Job Invoke(Action action)
        {
            var job = new Job(action);
            Enqueue(job);
            return job;
        }

        protected Job Invoke(Action<IProgress, CancellationToken> action)
        {
            var job = new Job(action);
            Enqueue(job);
            return job;
        }

        protected Job Interrupt(Action action)
        {
            var job = new Job(action);
            Interrupt(job);
            return job;
        }

        protected Job Interrupt(Action<IProgress, CancellationToken> action)
        {
            var job = new Job(action);
            Interrupt(job);
            return job;
        }

        protected Job<T> Invoke<T>(Func<T> func)
        {
            var job = new Job<T>(func);
            Enqueue(job);
            return job;
        }

        protected Job<T> Invoke<T>(Func<IProgress, CancellationToken, T> func)
        {
            var job = new Job<T>(func);
            Enqueue(job);
            return job;
        }

        protected Job<T> Interrupt<T>(Func<T> func)
        {
            var job = new Job<T>(func);
            Interrupt(job);
            return job;
        }

        protected Job<T> Interrupt<T>(Func<IProgress, CancellationToken, T> func)
        {
            var job = new Job<T>(func);
            Interrupt(job);
            return job;
        }


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
        /// デキュー
        /// </summary>
        /// <returns></returns>
        public IDispatchable Dequeue()
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

        /// <summary>
        /// 通常キュー登録
        /// </summary>
        /// <param name="job"></param>
        void Enqueue(IDispatchable job)
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
        void Interrupt(IDispatchable job)
        {
            lock (queue)
            {
                queue.Insert(0, job);
            }
        }
    }
}
