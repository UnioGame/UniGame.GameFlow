namespace UniGame.UniNodes.UiNodes.Runtime.Interfaces
{
    using NodeSystem.Runtime.Interfaces;
    using UniGreenModules.UniUiSystem.Runtime.Interfaces;
    using UniRx;

    public interface IUiModuleSlot : IUiPlacement
    {
        
        string SlotName { get; }

        IConnector<IMessagePublisher> Value { get; }
        
    }
}