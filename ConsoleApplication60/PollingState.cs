using System.Collections;
using System.ComponentModel;

namespace QueueMachine
{
    /// <summary>
    /// ポーリングステート（パッシブ用）
    /// </summary>
    public class PollingState : INotifyPropertyChanged
    {
        PropertyChangedEventArgs nameArgs = new PropertyChangedEventArgs(nameof(Name));
        public string Name { get; set; }

        object value;
        PropertyChangedEventArgs valueArgs = new PropertyChangedEventArgs(nameof(Value));
        public object Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                PropertyChanged?.Invoke(this, valueArgs);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        IEnumerator enumerator;
        public PollingState(string name, IEnumerator enumerator)
        {
            this.Name = name;
            this.enumerator = enumerator;
        }

        /// <summary>
        /// ポーリング
        /// </summary>
        public void Polling()
        {
            enumerator.MoveNext();
        }
    }
}
