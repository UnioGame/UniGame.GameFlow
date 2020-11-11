namespace UniGame.UniNodes.GameFlow.Runtime
{
    using System;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGameFlow.GameFlow.Runtime.Interfaces;
    using UniRx;
    using UnityEngine;

    /// <summary>
    /// base game service class for binding Context source data to service logic
    /// </summary>
    [Serializable]
    public abstract class GameService : IGameService, ICompletionSource
    {
        [SerializeField]
        protected readonly LifeTimeDefinition   lifeTimeDefinition = new LifeTimeDefinition();
        
        /// <summary>
        /// ready by default
        /// </summary>
        [SerializeField]
        protected readonly BoolReactiveProperty isReady            = new BoolReactiveProperty(false);

        /// <summary>
        /// complete service awaiter to mark it as ready
        /// </summary>
        public void Complete() => isReady.Value = true;
        
        /// <summary>
        /// terminate service lifeTime to release resources
        /// </summary>
        public void Dispose() => lifeTimeDefinition.Terminate();

        public bool IsComplete => isReady.Value;

        public ILifeTime LifeTime => lifeTimeDefinition.LifeTime;

        public IReadOnlyReactiveProperty<bool> IsReady => isReady;

    }
}
