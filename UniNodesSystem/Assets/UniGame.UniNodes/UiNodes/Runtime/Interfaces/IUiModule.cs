    namespace UniGame.UniNodes.UiNodes.Runtime.Interfaces
{
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniUiSystem.Runtime.Interfaces;

    public interface IUiModule : IUiView<IValueReceiver>
    {
        
        IContainer<IUiModuleSlot> Slots { get; }
        
        ITriggersContainer Triggers { get; }
        
        void AddTrigger(IInteractionTrigger trigger);
        
        void AddSlot(IUiModuleSlot slot);
    }
}