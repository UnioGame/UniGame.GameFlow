namespace Taktika.GameRuntime.Abstract
{
    using UniGreenModules.UniCore.Runtime.Interfaces;

    public interface IGameManager
    {
        
        IContext GameContext { get; }
        
    }
}