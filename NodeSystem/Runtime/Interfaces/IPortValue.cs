namespace UniGame.UniNodes.NodeSystem.Runtime.Interfaces
{
    using System;
    using System.Collections.Generic;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniRx;

    public interface IPortValue : 
        IContext,
        IConnector<IMessagePublisher>,
        INamedItem
    {
        IReadOnlyList<Type> ValueTypes { get; }
        
        IObservable<Unit> PortValueChanged { get; }

        bool IsValidPortValueType(Type type);

    }
}