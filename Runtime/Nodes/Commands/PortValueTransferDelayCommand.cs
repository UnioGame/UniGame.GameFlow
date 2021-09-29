namespace UniGame.UniNodes.Nodes.Runtime.Commands
{
    using System;
    using System.Collections;
    using Cysharp.Threading.Tasks;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniModules.UniRoutine.Runtime.Extension;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniRx;

    [Serializable]
    public class PortValueTransferDelayCommand : ILifeTimeCommand
    {
        private readonly float delay;
        private readonly PortValuePreTransferCommand transferCommand;
        
        public PortValueTransferDelayCommand(IPortValue input, IPortValue output, float delay)
        {
            this.delay = delay;
            transferCommand = new PortValuePreTransferCommand(DelayAction,input,input,output);
        }
        
        public UniTask Execute(ILifeTime lifeTime)
        {
            transferCommand.Execute(lifeTime);
            return UniTask.CompletedTask;
        }

        private IEnumerator DelayAction(IContext source,IMessagePublisher target)
        {
            yield return this.WaitForSeconds(delay);
        }
        
    }
}
