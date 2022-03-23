
using Commander.Core;
using System;
using Xunit;

namespace Commander.Tests
{
    public class TestCommand
    {
        public string Payload { get; set; }
    }
    public class TestCommandHandler : ICommandHandler<TestCommand>
    {
        public TestCommandHandler(Action<TestCommand> callback)
        {
            this.callback = callback;
        }
        private Action<TestCommand> callback;
        public void Handle(TestCommand command)
        {
            callback(command);
        }
    }
    public class CommandExecutorTests
    {
        public class SingleHandler
        {
            private CommandExecutor executor;
            public SingleHandler()
            {
                ServiceLocator locator = new ServiceLocator();
                locator.SetHandler(new TestCommandHandler((cmd) =>
                {
                    Assert.Equal("Test", cmd.Payload);
                }));
                executor = new CommandExecutor(locator);

            }
            [Fact]
            public void CallsHandler()
            {
                TestCommand command = new TestCommand() { Payload = "Test" };
                executor.Execute(command);
            }
        }
        public class NoHandler
        {
            private CommandExecutor executor;
            public NoHandler()
            {
                ServiceLocator locator = new ServiceLocator();
                executor = new CommandExecutor(locator);

            }
            [Fact]
            public void CommandExecutes()
            {
                TestCommand command = new TestCommand() { Payload = "Test" };
                executor.Execute(command);
            }
        }
        public class SingleHook
        {
            private CommandExecutor executor;
            public SingleHook()
            {
                ServiceLocator locator = new ServiceLocator();
                locator.
                executor = new CommandExecutor(locator);

            }
            [Fact]
            public void CommandExecutes()
            {
                TestCommand command = new TestCommand() { Payload = "Test" };
                executor.Execute(command);
            }
        }
    }
}