namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniCore.Runtime.ProfilerTools;
    using Core.Runtime;
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

        protected sealed override async UniTask OnValueUpdate(IContext context)
        {
            if(context == null) return;
            var isComplete = await OnContextActivate(context);
            if(isComplete) CompleteProcessing(context);
        }

        /// <summary>
        /// new context handler if return true, when context will be published into output
        /// </summary>
        protected virtual UniTask<bool> OnContextActivate(IContext context)  => UniTask.FromResult(true);
        
    }
}
