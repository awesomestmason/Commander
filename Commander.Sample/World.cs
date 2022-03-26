using Commander.Core;
using Commander.Core.CommandProvider;

public class World
{
    private List<Entity> entities = new();
    private CommandProvider provider;
    public CommandExecutor Executor { get; set; }
    private static World instance;
    public static World Instance => instance;
    public World(CommandProvider provider)
    {
        instance = this;
        this.provider = provider;

        Executor = new CommandExecutor(provider);

        SpawnEntities();
    }

    private void SpawnEntities()
    {
        Entity fish = new Entity() { Name = "Fish", Health = 5 };
        Entity cow = new Entity() { Name = "Cow", Health = 100 };
        Entity elephant = new Entity() { Name = "elephant", Health = 400 };

        entities.Add(cow);
        entities.Add(fish);
        entities.Add(elephant);

    }

    public void DamageRandom()
    {
        int index = Random.Shared.Next(entities.Count);
        Entity e = entities[index];
        DamageCommand cmd = new DamageCommand(e, 10);
        Executor.Execute(cmd);
    }
    
    public void PrintEntities()
    {
        foreach (Entity e in entities)
        {
            Console.WriteLine($"{e.Name}{(e.Dead ? "(DEAD)" : "")}: {e.Health}hp");
        }
    }
}
