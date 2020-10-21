namespace UniModules.UniGameFlow.GameFlow.Runtime.Services
{
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using Interfaces;
    using UniGame.Core.Runtime.Interfaces;
    using UniGame.Core.Runtime.ScriptableObjects;
    using UniModules.UniContextData.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;

    public interface IEmptyGameService : IGameService
    {
        
    }

    public abstract class ServiceDataSourceAsset : ServiceDataSourceAsset<IEmptyGameService>
    {
    }

    public abstract class ServiceDataSourceAsset<TApi> :
        LifetimeScriptableObject,
        IAsyncContextDataSource
        where TApi : class, IGameService
    {
        #region inspector

        public bool isSharedSystem = true;

        #endregion

        private        TApi          _sharedService;
        private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        #region public methods
    
        public async UniTask<IContext> RegisterAsync(IContext context)
        {
            var service = await CreateServiceAsync(context);
        
            service.IsReady.
                Where(x => x).
                Subscribe(x => context.Publish(service)).
                AddTo(context.LifeTime);

            return context;
        }
    
        /// <summary>
        /// service factory
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async UniTask<TApi> CreateServiceAsync(IContext context)
        {
            await _semaphoreSlim.WaitAsync();
            try {
                if (isSharedSystem && _sharedService == null) {
                    _sharedService = await CreateServiceInternalAsync(context);
                    _sharedService.AddTo(LifeTime);
                }
            }
            finally {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                _semaphoreSlim.Release();
            }

            var service = isSharedSystem ? _sharedService : (await CreateServiceInternalAsync(context)).AddTo(LifeTime);

            service.Bind(context);
            return service;
        }

        #endregion

        protected abstract UniTask<TApi> CreateServiceInternalAsync(IContext context);
    }
}