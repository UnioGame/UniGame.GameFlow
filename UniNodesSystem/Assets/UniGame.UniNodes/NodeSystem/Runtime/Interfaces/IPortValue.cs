namespace UniGreenModules.UniNodeSystem.Runtime.Interfaces
{
    using System;
    using UniCore.Runtime.Interfaces;
    using UniRx;

    public interface IPortValue : 
        IContext,
        IConnector<IMessagePublisher>,
        INamedItem
    {

        IObservable<Unit> PortValueChanged { get; }

    }
}