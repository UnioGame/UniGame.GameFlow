namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Nodes.Runtime.Nodes
{
    using System;
    using NodeSystem.Runtime.Attributes;
    using UniCore.Runtime.Interfaces;
    using UniCore.Runtime.ProfilerTools;
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
                Do(x => GameLog.Log($"{ItemName} : CONTEXT VALUE {x}")).
                Select(x => x.Receive<T>()).
                Switch();
        }
    }
}
