using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueMachine
{
    /// <summary>
    /// 実行処理(主体)
    /// </summary>
    /// <typeparam name="SERVICE"></typeparam>
    public class Worker : IServiceProvider, IInitializable, IDisposable, IRemovable
    {
        /// <summary>
        /// 生成されたサービス一覧
        /// </summary>
        List<IService> services = new List<IService>();
        List<object> passives = new List<object>();
        Task activeTask;
        Task passiveTask;
        bool isPower = false;

        /// <summary>
        /// サービス作成
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            var service = Activator.CreateInstance(serviceType, new object[] { this });
            services.Add((IService)service);
            return service;
        }

        /// <summary>
        /// サービス作成
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetService<T>() => (T)GetService(typeof(T));

        /// <summary>
        /// サービス解除
        /// </summary>
        /// <param name="queue"></param>
        public void Remove(object queue) => services.Remove((IService)queue);

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public void Initialize(object sender)
        {
            foreach (var service in services)
            {
                service.Initialize(sender);
            }
            isPower = true;
            passiveTask = Task.Run((Action)TaskPassiveMain);
            activeTask = Task.Run((Action)TaskActiveMain);
        }

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            isPower = false;
            foreach (var service in services)
            {
                service.Dispose();
            }
            activeTask = null;
            passiveTask= null;
        }

        /// <summary>
        /// タスクメイン(active)
        /// </summary>
        void TaskActiveMain()
        {
            isPower = true;
            while (isPower)
            {
                foreach (var service in services)
                {
                    if (!isPower) break;
                    var job = service.Dequeue();
                    job?.Dispatch();
                }
                Task.Delay(1).Wait();
            }
        }

        /// <summary>
        /// タスクメイン(passive)
        /// </summary>
        void TaskPassiveMain()
        {
            isPower = true;
            while (isPower)
            {
                foreach (var passive in passives)
                {
                    if (!isPower) break;
                    //処理実行
                }
                Task.Delay(1).Wait();
            }
        }
    }
}
