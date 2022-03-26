public class DamageCommand
{
    public float Amount { get; set; }
    public Entity Target { get; private set; }

    public DamageCommand(Entity target, float amount)
    {
        Amount = amount;
        Target = target;
    }
}
