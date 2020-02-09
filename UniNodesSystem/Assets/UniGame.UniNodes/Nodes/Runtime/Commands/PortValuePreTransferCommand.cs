namespace UniGreenModules.UniNodes.Runtime.Commands
{
    using System;
    using System.Collections;
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniCore.Runtime.Interfaces;
    using UniCore.Runtime.Rx.Extensions;
    using UniNodeSystem.Runtime.Interfaces;
    using UniRoutine.Runtime;
    using UniRoutine.Runtime.Extension;
    using UniRx;

    [Serializable]
    public class PortValuePreTransferCommand : ILifeTimeCommand,IContextWriter
    {
        private readonly Func<IContext,IMessagePublisher,IEnumerator> action;
        private readonly IConnector<IMessagePublisher> connector;
        private readonly IContext sourceContext;
        private readonly IMessagePublisher target;

        private RoutineHandler handler;

        public PortValuePreTransferCommand(
            Func<IContext,IMessagePublisher,IEnumerator> action,
            IConnector<IMessagePublisher> connector,
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
            connector.Connect(this).
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
