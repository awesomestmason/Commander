using System.Collections.Concurrent;
using Commander.Core.Util;
namespace Commander.Core;
public partial class CommandProvider : INotifyChangeCommandProvider
{
    //Map of all Command Handlers
    private readonly ConcurrentDictionary<Type, object> handlers = new();
    //Map of sorted list of all Command Hooks
    private readonly ConcurrentDictionary<Type, object> hooks = new();

    private Action<Type> notifyChangeHandler;
    public IEnumerable<ICommandHook<T>> GetCommandHooks<T>()
    {
        return (SortedList<CommandHookWrapper<T>>)hooks.GetOrAdd(typeof(T), (type) =>
        {
            return new SortedList<CommandHookWrapper<T>>(new HookComparer<T>());
        });
    }

    public ICommandHandler<T>? GetHandler<T>()
    {
        return (ICommandHandler<T>?) handlers.GetValueOrDefault(typeof(T));
    }

    public void SetHandler<T>(ICommandHandler<T> handler)
    {
        handlers[typeof(T)] = handler;
        notifyChangeHandler?.Invoke(typeof(T));
    }

    public IDisposable AddHook<T>(ICommandHook<T> hook, int order = 0)
    {
        var currentList = (SortedList<CommandHookWrapper<T>>) GetCommandHooks<T>();
        var newEntry = new CommandHookWrapper<T>(hook, currentList, notifyChangeHandler, order);
        currentList.Add(newEntry);
        notifyChangeHandler?.Invoke(typeof(T));
        return newEntry;
    }

    public void SetNotifyChange(Action<Type> typeChange)
    {
        notifyChangeHandler = typeChange;
    }
}
