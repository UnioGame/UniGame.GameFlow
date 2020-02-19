﻿namespace UniGame.UniNodes.Nodes.Runtime.Common
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
        /// <summary>
        /// subscribe to selected data from active context value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IObservable<T> Receive<T>()
        {
            return Source.
                Where(x => x != null).
                Select(x => x.Receive<T>()).
                Switch();
        }
        
        /// <summary>
        /// data publishing to current context
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public void Publish<T>(T data)
        {
            if (Source.Value == null) {
                GameLog.LogWarning($"You are try to Publish DATA {data} to {graph.name}:{ItemName} while context is NULL");
                return;
            }
            Source.Value.Publish(data);
        }

        protected override void OnExecute()
        {
            base.OnExecute();
            
            Source.Where(x => x!=null).
                Do(OnContextActivate).
                Subscribe().
                AddTo(lifeTime);
        }

        protected virtual void OnContextActivate(IContext context) { }


        
    }
}
