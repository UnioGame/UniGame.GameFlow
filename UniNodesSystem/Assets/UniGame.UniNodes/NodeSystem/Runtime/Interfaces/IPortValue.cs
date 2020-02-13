namespace UniGreenModules.UniNodeSystem.Runtime.Interfaces
{
    using System;
    using System.Collections.Generic;
    using UniCore.Runtime.Interfaces;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core.Interfaces;
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