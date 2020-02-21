namespace UniGame.UniNodes.GameFlow.Runtime
{
    using System;
    using Interfaces;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniRx;

    /// <summary>
    /// base game service class for binding Context source data to service logic
    /// </summary>
    [Serializable]
    public abstract class GameService : IGameService, ICompletionSource
    {
        protected readonly LifeTimeDefinition lifeTimeDefinition = new LifeTimeDefinition();

        protected readonly BoolReactiveProperty isReady = new BoolReactiveProperty(false);
        
        /// <summary>
        /// service local lifetime
        /// </summary>
        protected ILifeTime serviceLifeTime;

        public IContext Bind(IContext context, ILifeTime lifeTime = null)
        {
            serviceLifeTime = lifeTimeDefinition.LifeTime;
            lifeTime?.AddDispose(this);
            return OnBind(context, serviceLifeTime);
        }
        
        /// <summary>
        /// complete service awaiter to mark it as ready
        /// </summary>
        public void Complete()
        {
            isReady.Value = true;
        }
        
        /// <summary>
        /// terminate service lifeTime to release resources
        /// </summary>
        public void Dispose()
        {
            lifeTimeDefinition.Terminate();
        }

        public bool IsComplete => isReady.Value;

        public ILifeTime LifeTime => lifeTimeDefinition.LifeTime;

        public IReadOnlyReactiveProperty<bool> IsReady => isReady;

        protected abstract IContext OnBind(IContext context, ILifeTime lifeTime);


    }
}
