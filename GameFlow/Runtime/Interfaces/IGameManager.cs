namespace Taktika.GameRuntime.Abstract
{
    using UniModules.UniGame.Core.Runtime.Interfaces;

    public interface IGameManager
    {
        
        IContext GameContext { get; }
        
    }
}