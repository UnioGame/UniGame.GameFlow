namespace UniGame.UniNodes.Nodes.Runtime.Commands
{
    using System;
    using NodeSystem.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniRx;

    [Serializable]
    public class MessageBroadcastCommand : ILifeTimeCommand
    {
        private readonly IConnector<IMessagePublisher> _source;
        private readonly IMessagePublisher             _target;

        public MessageBroadcastCommand(IConnector<IMessagePublisher> source, IMessagePublisher target)
        {
            _source = source;
            _target = target;
        }
        
        public void Execute(ILifeTime lifeTime)
        {
            _source.Bind(_target).AddTo(lifeTime);
        }
    }
}