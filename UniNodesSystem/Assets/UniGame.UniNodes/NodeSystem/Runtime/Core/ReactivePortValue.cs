namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using System.Runtime.CompilerServices;
    using Interfaces;
    using Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.Attributes;
    using UniGreenModules.UniCore.Runtime.ProfilerTools;
    using UniRx;
    using UnityEngine;

    [Serializable]
    public class ReactivePortValue<TValue> : 
        IReactivePortValue<TValue>
    {
        #region inspector
        
        [SerializeField] public bool sendByBind = true;
        
        [SerializeField] public TValue value = default;

        [ReadOnlyValue]
        [SerializeField] public int nodeId;
        
        [ReadOnlyValue]
        [SerializeField] public string portName;
        
        #endregion
        
        [NonSerialized] protected INode node;  
                
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
        
        public void Bind(INode target, string name)
        {
            this.portName = name;
            Initialize(target);

            if (Application.isPlaying && sendByBind) {
                Publish(value);
            }
        }

        public void Initialize(INode target)
        {
            this.node = target;
            this.nodeId = target.Id;
            broker = GetBroker();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish(TValue portValue)
        {
            value = portValue;
            if (broker == null) {
                GameLog.LogError("Reactive Value must be initialized before use");
            }
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
                return null;
            }

            var port = node.GetPort(portName);
            var result = port?.Value;
            return result;
        }
    }
}


