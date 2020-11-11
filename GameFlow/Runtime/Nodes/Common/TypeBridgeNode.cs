namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using System;
    using NodeSystem.Runtime.Attributes;
    using NodeSystem.Runtime.Core;
    using NodeSystem.Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces.Rx;

    [HideNode]
    [Serializable]
    public class TypeBridgeNode<TData> : UniNode,
        IReadonlyRecycleReactiveProperty<TData>
    {
        private STypeBridgeNode<TData> value;

        public IDisposable Subscribe(IObserver<TData> observer) =>
            value.Subscribe(observer);

        public TData Value => value.Value;
        public bool HasValue => value.HasValue;
        
        protected override IProxyNode CreateInnerNode()
        {  
            value = new STypeBridgeNode<TData>(id, nodeName, ports);
            return value;
        }

    }
}