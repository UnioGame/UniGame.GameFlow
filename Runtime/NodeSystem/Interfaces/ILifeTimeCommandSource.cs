namespace UniModules.GameFlow.Runtime.Core.Interfaces
{
    using Runtime.Interfaces;
    using global::UniGame.Core.Runtime;

    public interface ILifeTimeCommandSource : IValidator
    {
        ILifeTimeCommand Create(IUniNode node);
        
        bool IsUpdatable { get; }
    }
}