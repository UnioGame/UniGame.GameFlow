namespace UniGame.UniNodes.UiNodes.Runtime.Interfaces
{
    using System;
    using UniGreenModules.UniCore.Runtime.Interfaces;

    public interface IInteractionTrigger :
        IObservable<IInteractionTrigger>, 
        INamedItem
    {
        
        bool IsActive { get; }
        
        void SetState(bool active);
        
    }
}