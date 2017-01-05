using System;
using System.Runtime.CompilerServices;

namespace QueueMachine
{
    /// <summary>
    /// ジョブ完了待ち
    /// </summary>
    public class JobAwaiter : INotifyCompletion
    {
        public bool IsCompleted => job.finEvent.WaitOne(0);
        Job job;

        /// <summary>
        /// コンストラクタ処理
        /// </summary>
        /// <param name="job"></param>
        /// <param name="token"></param>
        public JobAwaiter(Job job)
        {
            this.job = job;
        }

        /// <summary>
        /// 完了処理
        /// </summary>
        /// <param name="continuation">続きの処理</param>
        public void OnCompleted(Action continuation) => continuation();

        /// <summary>
        /// 結果を返す
        /// </summary>
        /// <returns>結果</returns>
        public void GetResult() => job.Wait();
    }

    /// <summary>
    /// ジョブ完了待ち
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JobAwaiter<T> : INotifyCompletion
    {
        public bool IsCompleted => job.finEvent.WaitOne(0);
        Job<T> job;

        /// <summary>
        /// コンストラクタ処理
        /// </summary>
        /// <param name="job"></param>
        /// <param name="token"></param>
        public JobAwaiter(Job<T> job)
        {
            this.job = job;
        }

        /// <summary>
        /// 完了処理
        /// </summary>
        /// <param name="continuation">続きの処理</param>
        public void OnCompleted(Action continuation)
        {
            continuation();
        }

        /// <summary>
        /// 結果を返す
        /// </summary>
        /// <returns>結果</returns>
        public T GetResult() => (T)job.GetResult();
    }
}
