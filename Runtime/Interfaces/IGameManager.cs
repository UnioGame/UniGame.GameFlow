namespace Taktika.GameRuntime.Abstract
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniModules.UniGame.Core.Runtime.Interfaces;

    public interface IGameManager : ILifeTimeContext, IDisposable
    {
        IContext GameContext { get; }

        UniTask Execute();
        
    }
}