namespace UniGame.UniNodes.Nodes.Runtime.Commands
{
    using System;
    using NodeSystem.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniRx;


    [Serializable]
    public class ContextBroadCastCommand<TTarget> : ILifeTimeCommand, IContextWriter
    {
        private readonly Action<TTarget> action;
        private readonly IConnector<IMessagePublisher> connector;

        public ContextBroadCastCommand(Action<TTarget> action,IConnector<IMessagePublisher> connector)
        {
            this.action = action;
            this.connector = connector;
        }
        
        public void Execute(ILifeTime lifeTime)
        {
            connector.Bind(this).AddTo(lifeTime);
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
