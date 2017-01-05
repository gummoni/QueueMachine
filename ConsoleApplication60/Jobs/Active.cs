using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueMachine
{
    /// <summary>
    /// アクティブ
    /// </summary>
    class Active : IDisposable
    {
        List<IDeueueable> services = new List<IDeueueable>();
        Task task;
        bool isPower = true;

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="sender"></param>
        public void Initialize(object sender)
        {
            foreach (var service in services)
            {
                service.Initialize(sender);
            }

            task = Task.Run((Action)TaskActiveMain);
        }

        /// <summary>
        /// 追加
        /// </summary>
        /// <param name="service"></param>
        public void Add(IDeueueable service)
        {
            services.Add(service);
        }

        /// <summary>
        /// 開放処理
        /// </summary>
        public void Dispose()
        {
            isPower = false;
        }

        /// <summary>
        /// タスクメイン(active)
        /// </summary>
        async void TaskActiveMain()
        {
            bool idleflg = true;
            isPower = true;
            while (isPower)
            {
                idleflg = true;
                //各プレゼンターを並列実行
                foreach (var service in services)
                {
                    if (!isPower) break;
                    var job = service.Dequeue();
                    if (null != job)
                    {
                        //処理実行
                        idleflg = false;
                        job.Dispatch();
                    }
                }

                if (idleflg)
                {
                    await Task.Delay(1);
                }
            }

            //開放処理
            foreach (var service in services)
            {
                service.Dispose();
            }
            services.Clear();
        }

    }
}
