namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Nodes.Runtime.Nodes
{
    using System;
    using NodeSystem.Runtime.Attributes;
    using UniCore.Runtime.Interfaces;
    using UniRx;

    [Serializable]
    [HideNode]
    public class ContextNode : 
        TypeBridgeNode<IContext>, 
        IMessageReceiver
    {
        public IObservable<T> Receive<T>()
        {
            return valueData.
                Where(x => x!=null).
                Select(x => x.Receive<T>()).
                Switch();
        }
    }
}
