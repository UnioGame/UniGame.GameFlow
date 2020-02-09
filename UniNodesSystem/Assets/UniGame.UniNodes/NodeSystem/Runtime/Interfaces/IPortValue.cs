namespace UniGreenModules.UniNodeSystem.Runtime.Interfaces
{
    using System;
    using UniCore.Runtime.Interfaces;
    using UniRx;

    public interface IPortValue : 
        ITypeData,
        IDisposable,
        IConnector<IContextWriter>,
        INamedItem
    {

        IObservable<Unit> PortValueChanged { get; }

    }
}