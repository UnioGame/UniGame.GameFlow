namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces.Rx;
    using UniRx;

    [Serializable]
    [HideNode]
    public class ContextNode : UniNode,
        IReadonlyRecycleReactiveProperty<IContext>, 
        IMessageBroker
    {
        private SContextNode contextNode;
        

        public IDisposable Subscribe(IObserver<IContext> observer) => 
            contextNode.Subscribe(observer);

        public IContext Value => contextNode.Value;
        
        public bool HasValue => contextNode.HasValue;

        public void Complete()
        {
            contextNode.Complete();
        }
        
        public void Publish<T>(T message) => contextNode.Publish(message);

        public IObservable<T> Receive<T>() => contextNode.Receive<T>();
        
        public IReadOnlyReactiveProperty<IContext> Source => contextNode.Source;
        
        protected override IProxyNode CreateInnerNode()
        {  
            contextNode = new SContextNode()
            {
                id = id,
                nodeName = nodeName,
                ports = ports
            };
            
            return contextNode;
        }
        
        protected override void OnExecute()
        {
            base.OnExecute();
            Source.Where(x => x != null)
                .ObserveOnMainThread()
                .Do(async context =>
                    await OnContextActivate(context)
                        .AttachExternalCancellation(LifeTime.AsCancellationToken()).SuppressCancellationThrow())
                .Subscribe()
                .AddTo(LifeTime);
        }

        protected virtual UniTask OnContextActivate(IContext context) { return UniTask.CompletedTask; }
    }
}
