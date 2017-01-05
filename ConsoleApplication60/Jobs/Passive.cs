using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace QueueMachine
{
    /// <summary>
    /// パッシブ
    /// </summary>
    class Passive : IDisposable
    {
        public ObservableCollection<PollingState> States { get; } = new ObservableCollection<PollingState>();
        Task task;
        bool isPower = true;

        /// <summary>
        /// コンストラクタ処理
        /// </summary>
        /// <param name="sender"></param>
        public Passive(object sender)
        {
            var type = sender.GetType();
            var methods = type.GetRuntimeMethods();
            foreach (var method in methods)
            {
                var args = method.GetGenericArguments();
                if (args.Length == 0)
                {
                    if (method.IsPrivate)
                    {
                        if (method.ReturnType == typeof(IEnumerable))
                        {
                            var inv = ((IEnumerable)method.Invoke(sender, null)).GetEnumerator();
                            var nameAttr = method.GetCustomAttribute<DisplayNameAttribute>();
                            var name = (null == nameAttr) ? method.Name : nameAttr.DisplayName;
                            var state = new PollingState(name, inv);
                            States.Add(state);
                        }
                    }
                }
            }

            if (0 < States.Count)
            {
                //Passiveがあれば起動
                task = Task.Run((Action)TaskPassiveMain);
            }
        }

        /// <summary>
        /// ステートを返す
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object this[string name] => States.FirstOrDefault(_=>_.Name == name);

        /// <summary>
        /// 開放処理
        /// </summary>
        public void Dispose()
        {
            isPower = false;
        }

        /// <summary>
        /// タスクメイン(passive)
        /// </summary>
        async void TaskPassiveMain()
        {
            isPower = true;
            //定周期実行（状態ポーリング）
            while (isPower)
            {
                foreach (var passive in States)
                {
                    if (!isPower) break;
                    passive.Polling();
                }
                await Task.Delay(1);
            }

            //開放処理
            States.Clear();
        }

    }
}
