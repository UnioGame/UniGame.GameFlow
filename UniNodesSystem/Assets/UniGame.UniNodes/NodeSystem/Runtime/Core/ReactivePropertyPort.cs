using UniRx;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core
{
    public class ReactivePropertyPort<TValue, TType>  : 
        ReactivePortValue<TValue,TType>
        where TValue : ReactiveProperty<TType>, new()
    {

        public new void Publish<T>(T message)
        {
            if(message is TType messageValue)
                value.SetValueAndForceNotify(messageValue);
        }
        
    }
}
