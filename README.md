## Commander
Commander is a lightweight, flexible and extensible command framework written in C#.

### Examples

#### Setup
The CommandProvider is responsible for mapping command types to their respective handlers, and hooks.
The CommandExecutor is responsible for executing commands.
```
using Commander.Core;

...

CommandProvider provider = new CommandProvider();
CommandExecutor Executor = new CommandExecutor(provider);
```

#### Creating a new command
To create a command, simply create a new class or struct.
```
public struct DamageCommand
{
    public float Amount { get; set; }
    public Entity Target { get; private set; }

    public DamageCommand(Entity target, float amount)
    {
        Amount = amount;
        Target = target;
    }
}

```
However, this command does not yet have a handler. Each type of command will only have a single handler. Let's add a handler for this type of command. To do this, create a new class that implements ICommandHandler\<T\> where T is the type of the command.
```
public class DamageCommandHandler : ICommandHandler<DamageCommand>
{
    public void Handle(DamageCommand command)
    {
        command.Target.Health -= command.Amount;
    }
}
```
The last step is to register the handler with with CommandProvider, which we can do by simply calling SetHandler.
```
provider.SetHandler(new DamageCommandHandler());
```
There is also a built in utility function that registers all handlers in a given Assembly. This will scan the assembly and add all concrete implementations of ICommandHandler\<T\> to the provider.
```
 ProviderHelper.AddHandlersFromAssembly(provider, typeof(Program).Assembly);
```
And we're done. Now if we execute the command, we can see it being handled.
```
Entity entity = new Entity() { Health = 100 };
DamageCommand command = new DamageCommand() { Target = entity, Amount = 20 };
executor.Execute(command);
Console.WriteLine(entity.Health); //Prints "80"
```
#### Hooks
Commander allows you to create command hooks, which are able to modify, execute code, or abort a command whenever a command is executed. A command can have multiple command hooks, but only one command handler.

Let's create a hook, based off the previous example. Simply implement the ICommandHook\<T\> interface, and override the Process method, making sure to call `next(command)` at the end which calls the next hook, or handler. If you omit the `next(command)` call, you are effectively cancelling the command, and no more hooks or handlers will be run.
```
public class DoubleAllDamageHook : ICommandHook<DamageCommand> {
  public void Process(DamageCommand command, Action<DamageCommand> next){
    command.Amount *= 2;
    next(command);
  }
}
```
The above hook makes it so all damage commands have their amount doubled. However, the hook is not yet registered with the provider. To do that simply call AddHook, optionally specifying the order that this hook is run in (lower order hooks are executed earlier).
```
IDisposable installedHook = provider.AddHook(new DoubleAllDamageHook(), order: 0);
```
Adding a hook returns an IDisposable, which we can dispose of to remove the hook. We can repeat this line as many times as we want, which would result in 2x, 4x, 8x, 16x... damage values being applied. Let's rerun our program with the hook added and see the result.
```
Entity entity = new Entity() { Health = 100 };
DamageCommand command = new DamageCommand() { Target = entity, Amount = 20 };
executor.Execute(command);
Console.WriteLine(entity.Health); //Prints "60" now, since damage amount was doubled.
```
Now everytime we execute a damage command, the amount will be doubled. And of course if we dispose of the hook with `installedHook.Dispose()` then it will not longer be doubled. 

##### Hook Use-Cases
- Logging (WIP)
```
public class DoubleAllDamageHook : ICommandHook { //Remove generic type parameters to install a "global" hook.
  public void Process(object command, Action<object> next){
    Console.WriteLine($"Executing Command: {typeof(command)}");
    next(command);
  }
}
```
- Cancelling commands
```
public class DontDamageInvincibleEntities : ICommandHook<DamageCommand> {
  public void Process(DamageCommand command, Action<DamageCommand> next){
    if(!command.Target.Entity.IsInvincible){
        return; //Command won't be handled!
    }
    next(command); //Only call next if the entity is allowed to take damage!
  }
}
```
- Modify commands
```
public class DoubleAllDamageHook : ICommandHook<DamageCommand> {
  public void Process(DamageCommand command, Action<DamageCommand> next){
    command.Amount *= 2;
    next(command);
  }
}
```
- Executing a hook after the command has been handled.
```
public class LogCommandResult : ICommandHook<DamageCommand> {
  public void Process(DamageCommand command, Action<DamageCommand> next){
    next(command); //Call next() first, to ensure the command has been fully handled before running the next line.
    Console.WriteLine($"Entity now has {command.Target.Health} health");
  }
}
```
