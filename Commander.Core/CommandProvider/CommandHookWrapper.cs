using Commander.Core.Util;
namespace Commander.Core;

public class CommandHookWrapper<T> : ICommandHook<T>, IDisposable
{
    private readonly ICommandHook<T> hook;
    private readonly SortedList<CommandHookWrapper<T>> currentList;
    private readonly Action<Type> notifyChange;
    public readonly int Order;
    public CommandHookWrapper(ICommandHook<T> hook, SortedList<CommandHookWrapper<T>> list, Action<Type> notifyChange, int order)
    {
        this.notifyChange = notifyChange;
        this.currentList = list;
        this.hook = hook;
        Order = order;
    }

    public void Process(T command, Action<T> next)
    {
        hook.Process(command, next);
    }

    public void Dispose()
    {
        notifyChange?.Invoke(typeof(T));
        currentList.Remove(this);
    }
}
