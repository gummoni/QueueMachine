using System;

namespace QueueMachine
{
    internal interface IDeueueable : IInitializable, IDisposable
    {
        IDispatchable Dequeue();
    }
}
