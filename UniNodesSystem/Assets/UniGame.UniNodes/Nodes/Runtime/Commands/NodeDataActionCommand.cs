﻿namespace UniGreenModules.UniNodeSystem.Nodes.Commands
{
    using System;
    using Runtime.Interfaces;
    using UniCore.Runtime.DataFlow;
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniCore.Runtime.Interfaces;
    using UniCore.Runtime.Rx.Extensions;

    [Serializable]
    public class NodeDataActionCommand : ILifeTimeCommand, IContextWriter
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
        
        public void Execute(ILifeTime lifeTime)
        {
            port.Bind(this).
                AddTo(lifeTime);
        }

        public bool Remove<TData>()
        {
            onRemove?.Invoke();
            return true;
        }

        public void Publish<TData>(TData data)
        {
            onAddData?.Invoke();
        }
        
        public void CleanUp()
        {
        }
    }
}
