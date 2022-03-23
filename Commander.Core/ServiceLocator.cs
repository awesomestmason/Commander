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
        return (IEnumerable<ICommandHook<T>>) hooks.GetOrAdd(typeof(T), (type) =>
        {
            return new LinkedList<ICommandHook<T>>();
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

    public IDisposable AddHook<T>(ICommandHook<T> hook)
    {

    }

    private class CommandHookWrapper<T> : ICommandHook<T>
    {
        private readonly ICommandHook<T> hook;

        public int Order { get; set; } = 0;

        public CommandHookWrapper(ICommandHook<T> hook)
        {
            this.hook = hook;
        }

        public void Process(T command, Action<T> next)
        {
            hook.Process(command, next);
        }
    }


}