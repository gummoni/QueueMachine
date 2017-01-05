namespace QueueMachine
{
    public interface IProgress
    {
        void Report(object value);
    }
}
