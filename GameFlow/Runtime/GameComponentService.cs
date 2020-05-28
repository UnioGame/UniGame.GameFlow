namespace UniGame.UniNodes.GameFlow.Runtime
{
    using Interfaces;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniRx;
    using UnityEngine;

    public class GameComponentService<TService> : MonoBehaviour, IGameService
        where TService : IGameService,new() 
    {
        protected TService Service = new TService();

        public void Dispose()
        {
            Service.Dispose();
        }

        public ILifeTime LifeTime => Service.LifeTime;

        public IReadOnlyReactiveProperty<bool> IsReady => Service.IsReady;


        public IContext Bind(IContext context, ILifeTime lifeTime = null)
        {
            return Service.Bind(context, lifeTime);
        }
        
        private void OnDestroy()
        {
            Dispose();
        }

    }
}
