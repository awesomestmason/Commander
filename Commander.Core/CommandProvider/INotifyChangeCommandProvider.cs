namespace Commander.Core.CommandProvider;

public interface INotifyChangeCommandProvider : ICommandProvider
{
    void SetNotifyChange(Action<Type> typeChange);
}