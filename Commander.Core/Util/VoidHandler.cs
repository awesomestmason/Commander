namespace Commander.Core.Util;

public class VoidHandler<T> : ICommandHandler<T>
{
    public void Handle(T command)
    {
    }
}