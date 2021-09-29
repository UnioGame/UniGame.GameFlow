namespace UniModules.GameFlow.Runtime.Core.Commands
{
    using System;
    using Cysharp.Threading.Tasks;
    using Runtime.Interfaces;
    using UniModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
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
        
        public UniTask Execute(ILifeTime lifeTime)
        {
            port.Receive<TTarget>().
                Subscribe(action).
                AddTo(lifeTime);
            return UniTask.CompletedTask;
        }
        
    }
}
