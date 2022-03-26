namespace Commander.Core.CommandProvider;

public class VoidHandler<T> : ICommandHandler<T>
{
    public void Handle(T command)
    {
    }
}