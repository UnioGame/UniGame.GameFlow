namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using System.Runtime.CompilerServices;
    using Interfaces;
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
        public UniBaseNode target;

        [SerializeField]
        public string portName;
        
        #endregion

        private IMessageBroker broker;
        
        #region public properties
        
        public Type ValueType => typeof(TValue);

        public TValue Value => value;

        public bool HasValue => target!=null;

        #endregion
        
        public void Bind(UniBaseNode node, string name)
        {
            this.target = node;
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
            return broker = broker ?? target.GetPort(portName);
        }
    }
}


