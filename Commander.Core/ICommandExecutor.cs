namespace Commander.Core
{
    public interface ICommandExecutor
    {
        public void Execute<T>(T command);
    }
}