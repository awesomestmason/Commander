using Commander.Core;

public static class Program
{

    public static void Main(string[] args)
    {
        CommandProvider provider = new CommandProvider();
        ProviderHelper.AddHandlersFromAssembly(provider, typeof(Program).Assembly);
        World w = new World(provider);

        while (true)
        {
            PrintOptions();
            var input = Console.ReadLine();
            switch(input)
            {
                case "1":
                    w.DamageRandom();
                    break;
                case "2":
                    var randomName = new string[] { "Cow", "Fish", "Elephant" }[Random.Shared.Next(3)];
                    var randomAdd = Random.Shared.NextSingle() * 10 - 5;
                    var randomMultiplier = Random.Shared.NextSingle() * 2;
                    provider.AddHook(new SampleHook(randomName,
                            randomAdd,
                            randomMultiplier),
                        order: 0);
                    Console.WriteLine($"Added new hook: When a {randomName} is damaged, the damage amount will be multiplied by {randomMultiplier:F2} and then increased by {randomAdd:F2}");
                    break;
            }
            w.PrintEntities();
            Console.WriteLine("---------------");
        }
    }
    
    private static void PrintOptions()
    {
        Console.WriteLine("Options [1-3]:\n1. Damage a random entity\n2. Add a random hook\n3. Print entities");
    }
}
