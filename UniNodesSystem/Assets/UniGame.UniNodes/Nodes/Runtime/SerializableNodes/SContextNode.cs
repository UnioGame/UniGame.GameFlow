namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using System;
    using System.Collections.Generic;
    using NodeSystem.Runtime.Attributes;
    using NodeSystem.Runtime.Core;
    using UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;

    [Serializable]
    [HideNode]
    public class SContextNode : 
        STypeBridgeNode<IContext>, 
        IMessageBroker
    {
        #region constructor

        public SContextNode(){}

        public SContextNode(
            int id,
            string name,
            NodePortDictionary ports) : base(id, name, ports){}

        #endregion
        
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
                GameLog.LogWarning($"You are try to Publish DATA {data} to {graph.ItemName}:{ItemName} while context is NULL");
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
