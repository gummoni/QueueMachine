using System;

namespace QueueMachine
{
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
