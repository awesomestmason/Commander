using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Core
{
    public class CommandExecutor : ICommandExecutor
    {
        private IServiceLocator serviceLocator;

        public CommandExecutor(IServiceLocator locator)
        {
            this.serviceLocator = locator;
        }
        private Action<T> buildPipeline<T>(ICommandHandler<T> handler, IEnumerable<ICommandHook<T>> hooks)
        {
            return hooks.Aggregate(handler.Handle, (next, current) => (cmd) => current.Process(cmd, next));

        }
        public void Execute<T>(T command)
        {
            var handler = serviceLocator.GetHandler<T>();
            var hooks = serviceLocator.GetCommandHooks<T>();
            Action<T> pipeline = buildPipeline(handler, hooks);
            pipeline(command);
        }
    }
}
