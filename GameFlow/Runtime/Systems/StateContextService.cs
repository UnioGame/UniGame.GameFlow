namespace UniModules.UniGameFlow.GameFlow.Runtime.Systems
{
    using System;
    using System.Collections.Generic;
    using UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniGreenModules.UniGame.SerializableContext.Runtime.Addressables;
    using UniGreenModules.UniGame.SerializableContext.Runtime.AssetTypes;
    using UniGreenModules.UniStateMachine.Runtime.Interfaces;
    using UniRx;
    using UniRx.Async;

    public abstract class StateContextService : 
        BaseServiceAsset<IObservable<IContext>>,
        IAsyncState<IDisposable>
    {
        #region inspector
        
        /// <summary>
        /// system context data sources
        /// </summary>
        public List<AssetReferenceContextContainer> contextSources = 
            new List<AssetReferenceContextContainer>();

        public List<AssetReferenceContextService> referenceServices = new List<AssetReferenceContextService>();

        public List<ContextService> services = new List<ContextService>();
        
        #endregion

        public async UniTask<IDisposable> Execute()
        {
            var source = await LoadDataSource();

            var disposable = await Execute(source);
            return disposable;
        }

        public void Release() => Exit();
        
        protected async UniTask<IObservable<IContext>> LoadDataSource()
        {
            var contextAssets = ClassPool.Spawn<List<ContextContainerAsset>>();
            
            if (contextSources?.Count <= 0) {
                GameLog.LogRuntime($"EMPTY context system sources {name}");
                return Observable.Empty<IContext>();
            }
            //load all context sources
            await contextSources.LoadAssetsTaskAsync(contextAssets, LifeTime);
            //merge all source to single observable
            return contextAssets.Merge();
        }

        protected sealed override  async UniTask<Unit> OnInitialize(IObservable<IContext> source)
        {
            //start reference services
            foreach (var referenceService in referenceServices) {
                var service = await referenceService.LoadAssetTaskAsync(LifeTime);
                await service.Execute(source);
                LifeTime.AddDispose(service);
            }

            //start direct services
            foreach (var service in services) {
                await service.Execute(source);
                LifeTime.AddDispose(service);
            }

            source.
                Subscribe(UpdateContext).
                AddTo(LifeTime);
            
            return Unit.Default;
        }
        
        protected virtual void UpdateContext(IContext context)
        {
        }
    }
}
