namespace UniGame.UniNodes.Nodes.Runtime.Commands
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using Core.Runtime;

    [Serializable]
    public class NodeDataActionCommand : 
        ILifeTimeCommand, 
        IContextWriter
    {
        private readonly IPortValue port;
        private readonly Action onAddData;
        private readonly Action onRemove;

        public NodeDataActionCommand(
            IPortValue port,
            Action onAddData,
            Action onRemove = null)
        {
            this.port = port;
            this.onAddData = onAddData;
            this.onRemove = onRemove;
        }
        
        public UniTask Execute(ILifeTime lifeTime)
        {
            port.Broadcast(this).AddTo(lifeTime);
            return UniTask.CompletedTask;
        }

        public bool Remove<TData>()
        {
            onRemove?.Invoke();
            return true;
        }

        public void Publish<TData>(TData data) => onAddData?.Invoke();
        
        public void CleanUp() { }
    }
}
