using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace QueueMachine
{
    /// <summary>
    /// 実行処理(主体)
    /// </summary>
    /// <typeparam name="SERVICE"></typeparam>
    public class Worker : IServiceProvider, IInitializable, IDisposable
    {
        /// <summary>
        /// 生成されたサービス一覧
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Passive passive;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Active active = new Active();

        /// <summary>
        /// 状態を返す
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object this[string name] => passive[name];

        /// <summary>
        /// 状態一覧を返す
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<PollingState> GetStates() => passive.States;

        /// <summary>
        /// サービス作成
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            var service = (IDeueueable)Activator.CreateInstance(serviceType, new object[] { this });
            active.Add(service);
            return service;
        }

        /// <summary>
        /// サービス作成(base.Initialize()の前に定義する事)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetService<T>() => (T)GetService(typeof(T));

        /// <summary>
        /// 初期化(初期化と解放は入れ子の関係)
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public virtual void Initialize(object sender)
        {
            //同層の初期化
            var type = GetType();
            passive = new Passive(this);
            active.Initialize(this);

            //下層の初期化
            var models = GetType().GetRuntimeProperties().ToArray();
            foreach (var model in models)
            {
                if (model.PropertyType.GetInterfaces().Contains(typeof(IInitializable)))
                {
                    ((IInitializable)model.GetValue(this)).Initialize(this);
                }
            }
        }

        /// <summary>
        /// 破棄(初期化と解放は入れ子の関係)
        /// </summary>
        public virtual void Dispose()
        {
            //下層の解放
            var models = GetType().GetRuntimeProperties().ToArray();
            foreach (var model in models)
            {
                if (model.PropertyType.GetInterfaces().Contains(typeof(IDisposable)))
                {
                    ((IDisposable)model.GetValue(this)).Dispose();
                }
            }

            //同層の解放
            passive.Dispose();
            active.Dispose();
        }

    }
}
