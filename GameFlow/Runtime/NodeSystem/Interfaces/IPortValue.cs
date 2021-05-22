namespace UniGame.UniNodes.NodeSystem.Runtime.Interfaces
{
    using System;
    using System.Collections.Generic;
    using UniModules.UniGame.Core.Runtime.Interfaces;
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