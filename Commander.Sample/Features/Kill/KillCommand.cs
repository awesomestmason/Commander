public class KillCommand
{
    public Entity Target { get; private set; }

    public KillCommand(Entity target)
    {
        Target = target;
    }
}
