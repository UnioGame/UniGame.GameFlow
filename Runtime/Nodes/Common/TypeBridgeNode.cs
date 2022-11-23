namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using System;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Interfaces;
    using Core.Runtime.Rx;

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
            value = new STypeBridgeNode<TData>()
            {
                id = id,
                nodeName = nodeName,
                ports = ports
            };
            
            return value;
        }

    }
}