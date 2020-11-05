namespace Taktika.GameRuntime.Abstract
{
    using Cysharp.Threading.Tasks;
    using UniModules.UniGame.Core.Runtime.Interfaces;

    public interface IGameManager : ILifeTimeContext
    {
        IContext GameContext { get; }

        UniTask Execute();
        
    }
}