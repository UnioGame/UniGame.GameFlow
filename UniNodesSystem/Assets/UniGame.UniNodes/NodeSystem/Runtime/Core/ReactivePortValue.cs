namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using UniGreenModules.UniGame.Core.Runtime.Rx;
    using UniRx;
    using UnityEngine;
    using UnityEngine.Serialization;

    [Serializable]
    public class ReactivePortValue<TValue> : 
        IReactivePortValue<TValue>
    {
        public int Y;
        
        [SerializeReference] 
        public RecycleReactiveProperty<TValue> value;

        #region concstructor
        
        public ReactivePortValue()
        {
            this.value = new RecycleReactiveProperty<TValue>();
        }
        
        public ReactivePortValue(TValue value)
        {
            this.value = new RecycleReactiveProperty<TValue>(value);
        }
        
        #endregion
        
        public void Publish<T>(T message)
        {
            if(message is TValue value)
                this.value.Value = value;
        }
    
        public IDisposable Subscribe(IObserver<TValue> observer) => value.Subscribe(observer);

        public TValue Value => value.Value;

        public bool HasValue => value.HasValue;

        public IDisposable Bind(IMessagePublisher connection)
        {
            return value.Subscribe(connection.Publish);
        }

    }
}
