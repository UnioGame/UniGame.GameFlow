namespace UniGame.UniNodes.Nodes.Runtime.Commands
{
    using System;
    using System.Collections;
    using NodeSystem.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniRoutine.Runtime;
    using UniModules.UniRoutine.Runtime.Extension;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniRx;

    [Serializable]
    public class PortValuePreTransferCommand : ILifeTimeCommand,IContextWriter
    {
        private readonly Func<IContext,IMessagePublisher,IEnumerator> action;
        private readonly IBroadcaster<IMessagePublisher>                connector;
        private readonly IContext                                     sourceContext;
        private readonly IMessagePublisher                            target;

        private RoutineHandle handler;

        public PortValuePreTransferCommand(
            Func<IContext,IMessagePublisher,IEnumerator> action,
            IBroadcaster<IMessagePublisher> connector,
            IContext sourceContext,
            IMessagePublisher target)
        {
            this.action = action;
            this.connector = connector;
            this.sourceContext = sourceContext;
            this.target = target;
        }
        
        public void Execute(ILifeTime lifeTime)
        {
            connector.Broadcast(this).
                AddTo(lifeTime);
            lifeTime.AddCleanUpAction(() => handler.Cancel());
        }

        public void Publish<T>(T message)
        {
            handler = OnPublish(message).Execute();
        }

        public bool Remove<TData>() => true;

        public void CleanUp() {}

        private IEnumerator OnPublish<T>(T message)
        {
            yield return action(sourceContext, target);
            target.Publish(message);
        }
    }
}
