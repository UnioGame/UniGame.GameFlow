using UniGreenModules.UniNodeSystem.Runtime.Interfaces;
using UniRx;

public interface IReactiveSource : 
    IMessagePublisher, 
    IConnector<IMessagePublisher>
{
}