namespace UniModules.GameFlow.Runtime.Interfaces
{
    using System;
    using System.Collections.Generic;
    using global::UniGame.Core.Runtime;
    using UniRx;

    public interface IPortValue : 
        IContext,
        IManagedBroadcaster<IMessagePublisher>,
        IDisposable,
        INamedItem
    {
        IReadOnlyList<Type> ValueTypes { get; }
        
        IObservable<Unit> PortValueChanged { get; }

        bool IsValidPortValueType(Type type);
    }
}