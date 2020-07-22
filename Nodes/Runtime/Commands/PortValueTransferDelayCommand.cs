namespace UniGame.UniNodes.Nodes.Runtime.Commands
{
    using System;
    using System.Collections;
    using NodeSystem.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniRoutine.Runtime.Extension;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
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
        
        public void Execute(ILifeTime lifeTime)
        {
            transferCommand.Execute(lifeTime);
        }

        private IEnumerator DelayAction(IContext source,IMessagePublisher target)
        {
            yield return this.WaitForSeconds(delay);
        }
        
    }
}
