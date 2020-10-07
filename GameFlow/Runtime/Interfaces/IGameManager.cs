namespace Taktika.GameRuntime.Abstract
{
    using UniModules.UniCore.Runtime.Interfaces;

    public interface IGameManager
    {
        
        IContext GameContext { get; }
        
    }
}