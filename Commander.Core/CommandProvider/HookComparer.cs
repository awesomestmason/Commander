namespace Commander.Core;

public class HookComparer<T> : IComparer<CommandHookWrapper<T>> 
{
    public int Compare(CommandHookWrapper<T>? x, CommandHookWrapper<T>? y)
    {
        //Hooks execute in reverse order, so sort descending order
        return (y?.Order ?? 0) - (x?.Order ?? 0);
    }
}
