using System.Collections.Concurrent;
using Commander.Core.Util;
namespace Commander.Core.CommandProvider;
public partial class CommandProvider : ICommandProvider
{

    private readonly ConcurrentDictionary<Type, object> handlers = new();
    private readonly ConcurrentDictionary<Type, object> hooks = new();
    public IEnumerable<ICommandHook<T>> GetCommandHooks<T>()
    {
        return (SortedList<CommandHookWrapper<T>>)hooks.GetOrAdd(typeof(T), (type) =>
        {
            return new SortedList<CommandHookWrapper<T>>(new HookComparer<T>());
        });
    }

    public ICommandHandler<T> GetHandler<T>()
    {
        return (ICommandHandler<T>) handlers.GetValueOrDefault(typeof(T), new VoidHandler<T>());
    }

    public void SetHandler<T>(ICommandHandler<T> handler)
    {
        handlers[typeof(T)] = handler;
    }

    public IDisposable AddHook<T>(ICommandHook<T> hook, int order = 0)
    {
        var currentList = (SortedList<CommandHookWrapper<T>>) GetCommandHooks<T>();
        var newEntry = new CommandHookWrapper<T>(hook, currentList, order);
        currentList.Add(newEntry);
        return newEntry;
    }


}
