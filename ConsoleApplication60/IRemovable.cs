namespace QueueMachine
{
    /// <summary>
    /// パイプライン解除インターフェイス
    /// </summary>
    public interface IRemovable
    {
        void Remove(object service);
    }

}
