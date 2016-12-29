using System;
using System.Collections.Generic;

namespace QueueMachine
{
    /// <summary>
    /// サービスベース
    /// </summary>
    /// <typeparam name="MODEL"></typeparam>
    public class ServiceBase<MODEL> : IService, IDisposable
    {
        List<Job> queue = new List<Job>();
        protected MODEL Model;

        public ServiceBase(MODEL model)
        {
            Model = model;
        }

        public virtual void Initialize(object sender)
        {
            //ユーザーがコントローラを実装していく
            //自動化案
            //Iservice属性のローカルフィールドでGetserviceする
            //Getserviceの引数１にModel内にあるMODEL属性を含むプロパティがあればそれを活用する
        }

        public virtual void Dispose()
        {
            ((IRemovable)Model).Remove(this);
        }

        protected Job Invoke(Action action)
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

        /// <summary>
        /// 通常キュー登録
        /// </summary>
        /// <param name="job"></param>
        void Enqueue(Job job)
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
        void Interrupt(Job job)
        {
            lock (queue)
            {
                queue.Insert(0, job);
            }
        }
    }
}
