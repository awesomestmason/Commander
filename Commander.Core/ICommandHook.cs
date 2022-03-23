namespace Commander.Core
{
    public interface ICommandHook<T>
    {
        void Process(T command, Action<T> next);
    }
}
