namespace UniModules.UniGameFlow.GameFlow.Runtime.Systems
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using Interfaces;
    using UniGame.Core.Runtime.ScriptableObjects;
    using UniGreenModules.UniContextData.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;
    

    public abstract class ContextService : ContextService<IGameService>
    {
        
    }
    
    public abstract class ContextService<TApi> : 
        DisposableScriptableObject, 
        IAsyncContextDataSource
        where TApi : class, IGameService
    {
        #region inspector
        
        public bool isSharedSystem = true;

        #endregion

        private TApi _sharedService;
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1,1);

        #region public methods
        

        public async UniTask<IContext> RegisterAsync(IContext context)
        {
            var service = await CreateServiceAsync(context);
            context.Publish(service);
            return context;
        }
        
        /// <summary>
        /// service factory
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async UniTask<TApi> CreateServiceAsync(IContext context)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                if (isSharedSystem && _sharedService == null) {
                    _sharedService = await CreateServiceInternalAsync(context);
                    LifeTime.AddDispose(_sharedService);
                    LifeTime.AddCleanUpAction(() => _sharedService = null);
                }
            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                semaphoreSlim.Release();
            }
            var service = isSharedSystem ? _sharedService : 
                (await CreateServiceInternalAsync(context)).AddTo(context.LifeTime);
            
            service.Bind(context);
            return service;
        }
        
        #endregion

        protected override void OnDispose() => _lifeTimeDefinition.Release();

        protected abstract UniTask<TApi> CreateServiceInternalAsync(IContext context);

    }
}
