namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using UniGreenModules.UniGame.Core.Runtime.Rx;
    using UniRx;
    using UnityEngine;
    using UnityEngine.Serialization;

    [Serializable]
    public class ReactivePortValue<TValue, TType> : 
        IReactivePortValue<TType>
        where TValue : class, IReactiveProperty<TType>
    {
        public int Y;
        
        [SerializeReference] 
        public TValue value;
        
        public void Publish<T>(T message)
        {
            if(message is TType value)
                this.value.Value = value;
        }
    
        public IDisposable Subscribe(IObserver<TType> observer) => value.Subscribe(observer);

        public TType Value => value.Value;

        public bool HasValue => value.HasValue;

        public IDisposable Bind(IMessagePublisher connection)
        {
            return value.Subscribe(connection.Publish);
        }

    }
}
