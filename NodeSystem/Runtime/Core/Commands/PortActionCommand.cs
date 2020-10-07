namespace UniGame.UniNodes.NodeSystem.Runtime.Core.Commands
{
    using System;
    using Runtime.Interfaces;
    using UniModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniModules.UniCore.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniRx;

    [Serializable]
    public class PortActionCommand<TTarget> : ILifeTimeCommand
    {
        private readonly IPortValue port;
        private readonly Action<TTarget> action;

        public PortActionCommand(Action<TTarget> action,IPortValue port)
        {
            this.action = action;
            this.port = port;
        }
        
        public void Execute(ILifeTime lifeTime)
        {
            port.Receive<TTarget>().
                Subscribe(action).
                AddTo(lifeTime);
        }
        
    }
}
