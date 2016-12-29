using System;

namespace QueueMachine
{
    public interface IService : IInitializable, IDisposable
    {
        Job Dequeue();
    }
}
