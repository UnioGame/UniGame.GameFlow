namespace UniModules.GameFlow.Runtime.Interfaces
{
    using System;
    using UniRx;

    public interface IGraphCancelationNode : IGraphPortNode
    {
        IObservable<Unit> CancelObservable { get; }
    }
}
