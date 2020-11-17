namespace UnityAsyncHelper.Utils
{
    /// <summary>
    /// Статусы выполнения потока
    /// </summary>
    public enum ThreadStatus
    {
        UnStarted,
        InProgress,
        Completed,
        
        Canceled
    }
}