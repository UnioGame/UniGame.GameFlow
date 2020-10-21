namespace UniGame.UniNodes.Examples.ContextNodes.SimpleServices.Runtime.Context
{
    using System;
    using Cysharp.Threading.Tasks;
    using global::Examples.ContextNodes.SimpleServices.Runtime.Context;
    using UniModules.UniContextData.Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UnityEngine;

    [Serializable]
    public class DemoGameData : 
        IDemoGameContext, 
        IAsyncContextDataSource
    {
        [Header("Current Game session status")]
        public DemoGameStatus gameStatus = new DemoGameStatus();

        public IDemoGameStatus GameStatus => gameStatus;

        public async UniTask<IContext> RegisterAsync(IContext context)
        {
            context.Publish(GameStatus);
            context.Publish<IDemoGameContext>(this);
            return context;
        }

    }
}
