namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using Interfaces;
    using UniRx;
    using UnityEngine;

    [Serializable]
    public class ReactivePortValue<TValue> : 
        IReactivePortValue<TValue>
    {
        [SerializeField]
        public bool sendByBind = true;
        
        [SerializeReference] 
        public TValue value = default;

        [SerializeReference] 
        public IMessageBroker target;
        
        public Type ValueType => typeof(TValue);

        public TValue Value => value;

        public bool HasValue => target!=null;

        public void Bind(IMessageBroker broker)
        {
            this.target = broker;
            if (sendByBind) {
                SetValue(value);
            }
        }
        
        public void SetValue(TValue portValue)
        {
            value = portValue;
            target?.Publish(value);
        }

        public IDisposable Subscribe(IObserver<TValue> observer)
        {
            if(target == null)
                return Disposable.Empty;
            
            return target.Receive<TValue>().
                Subscribe(observer);
        }

    }
}


