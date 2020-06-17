namespace UniGame.UniNodes.GameFlow.Runtime.Interfaces
{
    using System;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniRx;

    public interface IGameService : 
        IDisposable, 
        ILifeTimeContext, 
        IReactiveStatus
    {
        /// <summary>
        /// Bind to target context during lifetime
        /// if lifetime is null, use lifetime of context
        /// </summary>
        /// <param name="context">data context</param>
        /// <param name="lifeTime">lifetime object</param>
        /// <returns>service context</returns>
        IContext Bind(IContext context);

    }
}
