using System;
using System.Threading;

namespace QueueMachine
{
    /// <summary>
    /// 遅延処理
    /// </summary>
    public class Job : IDispatchable, IProgress
    {
        Action action;
        internal ManualResetEvent finEvent = new ManualResetEvent(false);
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        public DateTime UpdateTime { get; private set; }
        public object Progress { get; private set; }

        public bool IsCompleted => finEvent.WaitOne(0);
        public bool IsFaulted => null != Exception;
        public bool IsCanceled => tokenSource.IsCancellationRequested;
        public Exception Exception { get; private set; }

        /// <summary>
        /// コンストラクタ処理
        /// </summary>
        /// <param name="action"></param>
        public Job(Action action)
        {
            this.action = action;
        }

        /// <summary>
        /// コンストラクタ処理
        /// </summary>
        /// <param name="action"></param>
        public Job(Action<IProgress, CancellationToken> action)
        {
            this.action = () => action(this, tokenSource.Token);
        }

        /// <summary>
        /// 実行処理
        /// </summary>
        public void Dispatch()
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
            finally
            {
                finEvent.Set();
            }
        }

        /// <summary>
        /// キャンセル処理
        /// </summary>
        public void Cancel()
        {
            tokenSource.Cancel();
        }

        /// <summary>
        /// 完了待ち処理
        /// </summary>
        public void Wait()
        {
            finEvent.WaitOne();
            if (Exception != null)
            {
                throw Exception;
            }
        }

        /// <summary>
        /// 完了待ち処理
        /// </summary>
        /// <param name="timeout"></param>
        public void Wait(TimeSpan timeout)
        {
            finEvent.WaitOne(timeout);
        }

        /// <summary>
        /// 状況報告処理
        /// </summary>
        /// <param name="value"></param>
        public void Report(object value)
        {
            UpdateTime = DateTime.Now;
            Progress = value;
        }

        /// <summary>
        /// await処理取得
        /// </summary>
        /// <returns></returns>
        public JobAwaiter GetAwaiter() => new JobAwaiter(this);
    }

    /// <summary>
    /// 遅延処理
    /// </summary>
    public class Job<T> : IDispatchable, IProgress
    {
        Func<T> func;
        internal ManualResetEvent finEvent = new ManualResetEvent(false);
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        public object Progress { get; private set; }
        public DateTime UpdateTime { get; private set; }
        public T Result { get; private set; }

        public bool IsCompleted => finEvent.WaitOne(0);
        public bool IsFaulted => null != Exception;
        public bool IsCanceled => tokenSource.IsCancellationRequested;
        public Exception Exception { get; private set; }

        /// <summary>
        /// コンストラクタ処理
        /// </summary>
        /// <param name="func"></param>
        public Job(Func<T> func)
        {
            this.func = func;
        }

        /// <summary>
        /// コンストラクタ処理
        /// </summary>
        /// <param name="func"></param>
        public Job(Func<IProgress, CancellationToken, T> func)
        {
            this.func = () => func(this, tokenSource.Token);
        }

        /// <summary>
        /// 実行処理
        /// </summary>
        public void Dispatch()
        {
            Result = func();
            finEvent.Set();
        }

        /// <summary>
        /// キャンセル処理
        /// </summary>
        public void Cancel()
        {
            tokenSource.Cancel();
        }

        /// <summary>
        /// 完了待ち処理
        /// </summary>
        public void Wait()
        {
            finEvent.WaitOne();
            if (Exception != null)
            {
                throw Exception;
            }
        }

        /// <summary>
        /// 完了待ち処理
        /// </summary>
        /// <param name="timeout"></param>
        public void Wait(TimeSpan timeout)
        {
            finEvent.WaitOne(timeout);
        }

        /// <summary>
        /// 完了待ち＆結果を返す
        /// </summary>
        /// <returns></returns>
        public T GetResult()
        {
            Wait();
            return Result;
        }

        /// <summary>
        /// 状況報告処理
        /// </summary>
        /// <param name="value"></param>
        public void Report(object value)
        {
            UpdateTime = DateTime.Now;
            Progress = value;
        }

        /// <summary>
        /// await処理取得
        /// </summary>
        /// <returns></returns>
        public JobAwaiter<T> GetAwaiter() => new JobAwaiter<T>(this);
    }

}
