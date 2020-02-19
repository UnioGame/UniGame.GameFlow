namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using System;
    using NodeSystem.Runtime.Attributes;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;

    [Serializable]
    [HideNode]
    public class ContextNode : 
        TypeBridgeNode<IContext>, 
        IMessageBroker
    {
        public IObservable<T> Receive<T>()
        {
            return Source.
                Where(x => x != null).
                Select(x => x.Receive<T>()).
                Switch();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Source.Where(x => x!=null).
                Do(OnContextActivate).
                Subscribe().
                AddTo(lifeTime);
        }

        protected virtual void OnContextActivate(IContext context) { }

        public void Publish<T>(T data)
        {
            if (Source.Value == null) {
                GameLog.LogWarning($"You are try to Publish DATA {data} to {graph.name}:{ItemName} while context is NULL");
                return;
            }
            Source.Value.Publish(data);
        }
        
    }
}
