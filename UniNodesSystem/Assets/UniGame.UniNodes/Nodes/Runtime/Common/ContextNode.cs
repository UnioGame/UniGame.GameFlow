namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using System;
    using NodeSystem.Runtime.Attributes;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniRx;

    [Serializable]
    [HideNode]
    public class ContextNode : 
        TypeBridgeNode<IContext>, 
        IMessageReceiver
    {
        public IObservable<T> Receive<T>()
        {
            return Source.
                Where(x => x != null).
                Select(x => x.Receive<T>()).
                Switch();
        }
    }
}
