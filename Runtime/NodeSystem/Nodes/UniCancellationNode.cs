namespace UniModules.GameFlow.Runtime.Core
{
    using System;
    using Runtime.Interfaces;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UniRx;

    [CreateNodeMenu("Common/EndPoint","EndPoint")]
    public class UniCancellationNode : UniPortNode, IGraphCancelationNode
    {
        public IObservable<Unit> CancelObservable => Output.PortValueChanged;
    }
    
}
