using Commander.Core.Util;
namespace Commander.Core.CommandProvider;

public class CommandHookWrapper<T> : ICommandHook<T>, IDisposable
{
    private readonly ICommandHook<T> hook;
    private readonly SortedList<CommandHookWrapper<T>> currentList;
    public readonly int Order;
    public CommandHookWrapper(ICommandHook<T> hook, SortedList<CommandHookWrapper<T>> list, int order)
    {
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
        currentList.Remove(this);
    }
}
