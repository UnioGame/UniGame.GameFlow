namespace UniGame.UniNodes.GameFlow.Runtime
{
    using System;
    using Interfaces;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniRx;

    [Serializable]
    public abstract class GameService : IGameService
    {
        protected LifeTimeDefinition lifeTimeDefinition = new LifeTimeDefinition();
        
        protected BoolReactiveProperty isReady = new BoolReactiveProperty(false);

        public IContext Bind(IContext context, ILifeTime lifeTime = null)
        {
            var bindLifeTime = lifeTime ?? context.LifeTime;
            return OnBind(context, bindLifeTime);
        }
        
        public void Dispose()
        {
            lifeTimeDefinition.Terminate();
        }

        public ILifeTime LifeTime => lifeTimeDefinition.LifeTime;

        public IReadOnlyReactiveProperty<bool> IsReady => isReady;

        protected abstract IContext OnBind(IContext context, ILifeTime lifeTime);
    }
}
