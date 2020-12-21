namespace UniGame.UniNodes.Nodes.Runtime.Commands
{
    using System;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniRx;

    [Serializable]
    public class ContextBroadcastCommand<TTarget> : ILifeTimeCommand, IContextWriter
    {
        private readonly Action<TTarget> _action;
        private readonly IBinder<IMessagePublisher> _connector;

        public ContextBroadcastCommand(Action<TTarget> action, IBinder<IMessagePublisher> connector)
        {
            _action = action;
            _connector = connector;
        }
        
        public void Execute(ILifeTime lifeTime)
        {
            _connector.Bind(this).AddTo(lifeTime);
        }

        public void Publish<T>(T message)
        {
            if (message is TTarget data) {
                _action(data);
            }
        }

        public bool Remove<TData>() => true;

        public void CleanUp() {}
    }
}
