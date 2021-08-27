namespace UniModules.GameFlow.Runtime.Core.Interfaces
{
    using Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;

    public interface ILifeTimeCommandSource : IValidator
    {
        ILifeTimeCommand Create(IUniNode node);
        
        bool IsUpdatable { get; }
    }
}