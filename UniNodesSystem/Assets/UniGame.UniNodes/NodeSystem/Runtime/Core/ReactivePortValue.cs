namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using System.Runtime.CompilerServices;
    using Interfaces;
    using UniCore.Runtime.ProfilerTools;
    using UniNodeSystem.Runtime.Interfaces;
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
        public string portName;
        
        #endregion
        
        protected INode node;  
                
        #region constructor
        
        public ReactivePortValue(){}

        public ReactivePortValue(TValue value)
        {
            this.value = value;
        }
        
        #endregion

        private IMessageBroker broker;
        
        #region public properties
        
        public Type ValueType => typeof(TValue);

        public TValue Value => value;

        public bool HasValue => node!=null;

        #endregion
        
        public void Bind(INode node, string name)
        {
            Initialize(node);

            this.portName = name;

            if (Application.isPlaying && sendByBind) {
                SetValue(value);
            }
        }

        public void Initialize(INode target)
        {
            this.node = target;
            this.nodeId = target.Id;
            broker = GetBroker();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(TValue portValue)
        {
            value = portValue;
            broker.Publish(value);
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
            if (node == null) {
                GameLog.LogError($"NULL Node at ReactivePoort {this.node} node id:{nodeId} {portName}");
            }
            var result = node.GetPort(portName).Value;
            return result;
        }
    }
}


