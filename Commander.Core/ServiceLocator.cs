using System.Collections.Concurrent;

namespace Commander.Core;

public interface IServiceLocator
{
    ICommandHandler<T> GetHandler<T>();
    IEnumerable<ICommandHook<T>> GetCommandHooks<T>();
}
public class ServiceLocator : IServiceLocator
{
    private class VoidHandler<T> : ICommandHandler<T>
    {
        public void Handle(T command)
        {
        }
    }

    private readonly ConcurrentDictionary<Type, object> handlers = new();
    private readonly ConcurrentDictionary<Type, object> hooks = new();
    public IEnumerable<ICommandHook<T>> GetCommandHooks<T>()
    {
        var list = (LinkedList<CommandHookWrapper<T>>)hooks.GetOrAdd(typeof(T), (type) =>
        {
            return new LinkedList<CommandHookWrapper<T>>();
        });
        return list;
    }

    public ICommandHandler<T> GetHandler<T>()
    {
        return (ICommandHandler<T>) handlers.GetValueOrDefault(typeof(T), new VoidHandler<T>());
    }

    public void SetHandler<T>(ICommandHandler<T> handler)
    {
        handlers[typeof(T)] = handler;
    }

    public IDisposable AddHook<T>(ICommandHook<T> hook)
    {
        var currentList = (LinkedList<CommandHookWrapper<T>>) GetCommandHooks<T>();
        var newEntry = new CommandHookWrapper<T>(hook, currentList);
        //Insert high values at front of list

        return newEntry;

        
    }

    private class CommandHookWrapper<T> : ICommandHook<T>, IDisposable
    {
        private readonly ICommandHook<T> hook;
        private readonly LinkedList<CommandHookWrapper<T>> currentList;

        public CommandHookWrapper(ICommandHook<T> hook, LinkedList<CommandHookWrapper<T>> list)
        {
            this.currentList = list;
            this.hook = hook;
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


}