namespace Commander.Core;

public interface INotifyChangeCommandProvider : ICommandProvider
{
    void SetNotifyChange(Action<Type> typeChange);
}