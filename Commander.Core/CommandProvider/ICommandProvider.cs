namespace Commander.Core;

/// <summary>
/// Responsible for providing Command Handler and Hooks to the <see cref="CommandExecutor">Executor</see>
/// </summary>
public interface ICommandProvider
{
    ICommandHandler<T>? GetHandler<T>();
    IEnumerable<ICommandHook<T>> GetCommandHooks<T>();
}
