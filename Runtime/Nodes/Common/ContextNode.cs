namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using Core.Runtime;
    using Core.Runtime.Rx;
    using UniRx;

    [Serializable]
    [HideNode]
    public class ContextNode : UniNode,
        IReadonlyRecycleReactiveProperty<IContext>, 
        IMessageBroker
    {
        private SContextNode contextNode;
        
        public IDisposable Subscribe(IObserver<IContext> observer) => contextNode.Subscribe(observer);

        public IContext Value => contextNode.Value;
        
        public bool HasValue => contextNode.HasValue;

        public void CompleteProcessing(IContext context) => contextNode.CompleteProcessing(context);
        
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
        
        protected override UniTask OnExecute()
        {
            Source.Where(x => x != null)
                .Do(context => OnContextActivate(context)
                    .AttachExternalCancellation(LifeTime.AsCancellationToken())
                    .SuppressCancellationThrow()
                    .Forget())
                .Subscribe()
                .AddTo(LifeTime);
            
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnContextActivate(IContext context) { return UniTask.CompletedTask; }
    }
}
