using Commander.Core;

public class DamageCommandHandler : ICommandHandler<DamageCommand>
{
    public void Handle(DamageCommand command)
    {
        command.Target.Health -= command.Amount;
        if(command.Target.Health < 0)
        {
            World.Instance.Executor.Execute(new KillCommand(command.Target));
        }
    }
}
