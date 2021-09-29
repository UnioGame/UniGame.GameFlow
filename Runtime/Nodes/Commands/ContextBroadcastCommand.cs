namespace UniGame.UniNodes.Nodes.Runtime.Commands
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniRx;


    [Serializable]
    public class ContextBroadCastCommand<TTarget> : ILifeTimeCommand, IContextWriter
    {
        private readonly Action<TTarget> action;
        private readonly IBroadcaster<IMessagePublisher> connector;

        public ContextBroadCastCommand(Action<TTarget> action,IBroadcaster<IMessagePublisher> connector)
        {
            this.action = action;
            this.connector = connector;
        }
        
        public UniTask Execute(ILifeTime lifeTime)
        {
            connector.Broadcast(this).AddTo(lifeTime);
            return UniTask.CompletedTask;
        }

        public void Publish<T>(T message)
        {
            if (message is TTarget data) {
                action(data);
            }
        }

        public bool Remove<TData>() => true;

        public void CleanUp() {}
    }
}
