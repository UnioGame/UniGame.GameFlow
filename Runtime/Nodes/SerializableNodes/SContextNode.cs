namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using System;
    using System.Collections.Generic;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniModules.GameFlow.Runtime.Core;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniRx;

    [Serializable]
    [HideNode]
    public class SContextNode : 
        STypeBridgeNode<IContext>, 
        IMessageBroker
    {
        /// <summary>
        /// subscribe to selected data from active context value
        /// </summary>
        /// <typeparam name="T"></typeparam>
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
                GameLog.LogWarning($"You are try to Publish DATA {data} to {GraphData.ItemName}:{ItemName} while context is NULL");
                return;
            }
            Source.Value.Publish(data);
        }


        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);
                        
            Source.Where(x => x!=null).
                Do(OnContextActivate).
                Subscribe().
                AddTo(LifeTime);
        }

        protected virtual void OnContextActivate(IContext context) { }
        
    }
}
