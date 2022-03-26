using Commander.Core.CommandProvider;

namespace Commander.Core
{
    public class CommandExecutor : ICommandExecutor
    {
        private ICommandProvider provider;

        public CommandExecutor(ICommandProvider provider)
        {
            this.provider = provider;
        }
        private Action<T> buildPipeline<T>(ICommandHandler<T> handler, IEnumerable<ICommandHook<T>> hooks)
        {
            return hooks.Aggregate(handler.Handle, (next, current) => (cmd) => current.Process(cmd, next));

        }
        public void Execute<T>(T command)
        {
            var handler = provider.GetHandler<T>();
            var hooks = provider.GetCommandHooks<T>();
            Action<T> pipeline = buildPipeline(handler, hooks);
            pipeline(command);
        }
    }
}
