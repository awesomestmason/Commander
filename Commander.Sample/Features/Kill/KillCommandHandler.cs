using Commander.Core;

public class KillCommandHandler : ICommandHandler<KillCommand>
{
    public void Handle(KillCommand command)
    {
        command.Target.Dead = true;
    }
}