
using Commander.Core;
using FluentAssertions;
using System;
using Xunit;
using Commander.Core.CommandProvider;

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
    public class TestCommandHook : ICommandHook<TestCommand>
    {
        private Action<TestCommand, Action<TestCommand>> callback;
        public TestCommandHook(Action<TestCommand, Action<TestCommand>> callback)
        {
            this.callback = callback;
        }
        public void Process(TestCommand command, Action<TestCommand> next)
        {
            callback(command, next);
        }
    }

    public class CommandExecutorTests
    {
        public class SingleHandler
        {
            private CommandExecutor executor;
            public SingleHandler()
            {
                CommandProvider locator = new CommandProvider();
                locator.SetHandler(new TestCommandHandler((cmd) =>
                {
                    cmd.Payload.Should().Be("Test");
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
                CommandProvider locator = new CommandProvider();
                executor = new CommandExecutor(locator);

            }
            [Fact]
            public void CommandExecutes()
            {
                TestCommand command = new TestCommand() { Payload = "Test" };
                executor.Execute(command);
            }
        }
        public class HookTests
        {
            private CommandExecutor executor;
            private CommandProvider locator;
            private bool hit;

            public HookTests()
            {
                TestCommandHook hook = new TestCommandHook((cmd, next) =>
                {
                    cmd.Payload.Should().Be("Test");
                    hit = true;
                    next(cmd);
                });
                locator = new CommandProvider();
                locator.AddHook(hook);
                executor = new CommandExecutor(locator);

            }
            [Fact]
            public void HookExecutes()
            {
                hit = false;
                TestCommand command = new TestCommand() { Payload = "Test" };
                executor.Execute(command);

                hit.Should().BeTrue();
            }
            [Fact]
            public void HookExecutesBeforeHandler()
            {
                hit = false;
                locator.SetHandler(new TestCommandHandler((cmd) =>
                {
                    hit.Should().BeTrue();
                    cmd.Payload.Should().Be("Test");
                }));

                TestCommand command = new TestCommand() { Payload = "Test" };
                executor.Execute(command);
                hit.Should().BeTrue();
            }

            [Fact]
            public void HooksExecuteInOrder()
            {
                int Counter = 0;
                hit = false;
                TestCommand command = new TestCommand() { Payload = "Test" };
                locator.AddHook(new TestCommandHook((cmd, next) =>
                {
                    cmd.Payload.Should().Be("Test");
                    Counter.Should().Be(2);
                    Counter++;
                    hit = true;
                    next(cmd);
                }), 3);
                locator.AddHook(new TestCommandHook((cmd, next) =>
                {
                    cmd.Payload.Should().Be("Test");
                    Counter.Should().Be(0);
                    Counter++;
                    hit = true;
                    next(cmd);
                }), 1);
                locator.AddHook(new TestCommandHook((cmd, next) =>
                {
                    cmd.Payload.Should().Be("Test");
                    Counter.Should().Be(1);
                    Counter++;
                    hit = true;
                    next(cmd);
                }), 2);

                Counter.Should().Be(0);
                executor.Execute(command);
                Counter.Should().Be(3);

                hit.Should().BeTrue();
            }

            [Fact]
            public void HooksDisposeProperly()
            {
                int Counter = 0;
                TestCommand command = new TestCommand() { Payload = "Test" };
                var hook = locator.AddHook(new TestCommandHook((cmd, next) =>
                {
                    cmd.Payload.Should().Be("Test");
                    Counter.Should().Be(0);
                    Counter++;
                    hit = true;
                    next(cmd);
                }), order: 3);

                Counter.Should().Be(0);
                executor.Execute(command);
                Counter.Should().Be(1);
                hook.Dispose();
                executor.Execute(command);
                Counter.Should().Be(1);
                hit.Should().BeTrue();
            }
        }
    }
}