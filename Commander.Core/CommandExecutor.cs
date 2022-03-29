using Commander.Core.Util;
using System.Collections.Concurrent;
namespace Commander.Core
{
    public class CommandExecutor : ICommandExecutor
    {
        private ICommandProvider provider;
        private ConcurrentDictionary<Type, object> pipelines = new();
        private ConcurrentDictionary<Type, bool> needsUpdate = new();
        private bool useCache = false;
        public CommandExecutor(ICommandProvider provider)
        {
            this.provider = provider;
            INotifyChangeCommandProvider? notifyProvider = provider as INotifyChangeCommandProvider;
            if(notifyProvider != null)
            {
                useCache = true;
                notifyProvider.SetNotifyChange((type) =>
                {
                    needsUpdate[type] = true;
                });
            }
        }
        private Action<T> buildPipeline<T>(ICommandHandler<T> handler, IEnumerable<ICommandHook<T>> hooks)
        {
            return hooks.Aggregate(handler.Handle, (next, current) => (cmd) => current.Process(cmd, next));

        }

        private Action<T> buildPipeline<T>()
        {
            var handler = provider.GetHandler<T>();
            if (handler == null)
            {
                handler = new VoidHandler<T>();
            }
            var hooks = provider.GetCommandHooks<T>();
            return buildPipeline(handler, hooks);
        }
        private Action<T> getPipeline<T>()
        {
            if (!useCache)
            {
                return buildPipeline<T>();
            }
            bool needsToUpdate = needsUpdate.GetValueOrDefault(typeof(T), false);
            needsUpdate[typeof(T)] = false;
            if (needsToUpdate)
            {
                Action<T> pipeline = buildPipeline<T>();
                pipelines[typeof(T)] = pipeline;
                return pipeline;
            }
            return (Action<T>) pipelines.GetOrAdd(typeof(T), (type) => buildPipeline<T>());
        }
        public void Execute<T>(T command)
        {
            Action<T> pipeline = getPipeline<T>();
            pipeline(command);
        }
    }
}
