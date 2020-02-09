namespace UniGreenModules.UniUiNodes.Runtime.Interfaces
{
    using UniNodeSystem.Runtime.Interfaces;
    using UniRx;
    using UniUiSystem.Runtime.Interfaces;

    public interface IUiModuleSlot : IUiPlacement
    {
        
        string SlotName { get; }

        IConnector<IMessagePublisher> Value { get; }
        
    }
}