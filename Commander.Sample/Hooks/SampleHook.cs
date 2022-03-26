using Commander.Core;

public class SampleHook : ICommandHook<DamageCommand>
{
    private readonly string targetName;
    private readonly float hpAdd;
    private readonly float hpMultiply;

    public SampleHook(string targetName, float hpAdd, float hpMultiply)
    {
        this.targetName = targetName;
        this.hpAdd = hpAdd;
        this.hpMultiply = hpMultiply;
    }
    public void Process(DamageCommand command, Action<DamageCommand> next)
    {
        if(command.Target.Name == targetName)
        {
            command.Amount *= hpMultiply;
            command.Amount += hpAdd;

        }
        next(command);
    }
}