namespace UniGame.GameRuntime.Abstract
{
    using System;
    using Cysharp.Threading.Tasks;
    using Core.Runtime;

    public interface IGameManager : ILifeTimeContext, IDisposable
    {
        IContext GameContext { get; }

        UniTask Execute();
        
        void Destroy();

    }
}