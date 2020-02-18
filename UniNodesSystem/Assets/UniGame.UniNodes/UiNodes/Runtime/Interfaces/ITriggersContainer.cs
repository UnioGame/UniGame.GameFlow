namespace UniGame.UniNodes.UiNodes.Runtime.Interfaces
{
    using System;
    using UniGreenModules.UniCore.Runtime.Interfaces;

    public interface ITriggersContainer : IContainer<IInteractionTrigger>
    {
        IObservable<IInteractionTrigger> TriggersObservable { get; }
        
    }
}