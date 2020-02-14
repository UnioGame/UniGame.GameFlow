namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using System.Runtime.CompilerServices;
    using Interfaces;
    using UniCore.Runtime.ProfilerTools;
    using UniNodeSystem.Nodes;
    using UniNodeSystem.Runtime.Core;
    using UniRx;
    using UnityEngine;

    [Serializable]
    public class ReactivePortValue<TValue> : 
        IReactivePortValue<TValue>
    {
        #region inspector
        
        [SerializeField]
        public bool sendByBind = true;
        
        [SerializeField]
        public TValue value = default;

        [SerializeField]
        public int nodeId;
        
        [SerializeField] 
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineEditor()]
#endif
        public NodeGraph graph;

        [SerializeField]
        public string portName;
        
        #region constructor
        
        public ReactivePortValue(){}

        public ReactivePortValue(TValue value)
        {
            this.value = value;
        }
        
        #endregion
        
        
        #endregion

        private IMessageBroker broker;
        
        #region public properties
        
        public Type ValueType => typeof(TValue);

        public TValue Value => value;

        public bool HasValue => graph!=null;

        #endregion
        
        public void Bind(NodeGraph graph,int id, string name)
        {
            this.graph = graph;
            this.nodeId = id;
            this.portName = name;
            
            if (Application.isPlaying && sendByBind) {
                SetValue(value);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(TValue portValue)
        {
            value = portValue;
            GetBroker()?.Publish(value);
        }

        public IDisposable Subscribe(IObserver<TValue> observer)
        {
            var receiver = GetBroker();
            if(receiver == null)
                return Disposable.Empty;
            
            return receiver.Receive<TValue>().
                Subscribe(observer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IMessageBroker GetBroker()
        {
            if (broker != null)
                return broker;
            var node = graph.GetNode(nodeId);
            if (node == null) {
                GameLog.LogError($"NULL Node at ReactivePoort {graph} node id:{nodeId} {portName}");
            }
            broker = node.GetPort(portName).Value;
            return broker;
        }
    }
}


