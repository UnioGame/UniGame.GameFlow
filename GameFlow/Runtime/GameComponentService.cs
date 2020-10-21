namespace UniGame.UniNodes.GameFlow.Runtime
{
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGameFlow.GameFlow.Runtime.Interfaces;
    using UniRx;
    using UnityEngine;

    public class GameComponentService<TService> : MonoBehaviour, IGameService
        where TService : IGameService,new() 
    {
        protected TService Service = new TService();

        public void Dispose() => Service.Dispose();

        public ILifeTime LifeTime => Service.LifeTime;

        public IReadOnlyReactiveProperty<bool> IsReady => Service.IsReady;
        
        public IContext Bind(IContext context) => Service.Bind(context);
        public void Complete()
        {
        }

        private void OnDestroy() => Dispose();
    }
}
