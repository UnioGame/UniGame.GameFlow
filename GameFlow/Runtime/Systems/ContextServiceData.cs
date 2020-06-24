namespace UniModules.UniGameFlow.GameFlow.Runtime.Systems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using UniCore.Runtime.ProfilerTools;
    using UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniGame.SerializableContext.Runtime.Addressables;
    using UniGame.SerializableContext.Runtime.AssetTypes;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime;
    using UniGreenModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniGreenModules.UniGame.SerializableContext.Runtime.Addressables;
    using UniRx;
    using UniRx.Async;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class ContextServiceData
    {
        public float loadTimeout = 10f;
        
        /// <summary>
        /// system context data sources
        /// </summary>
        public List<AssetReferenceContextContainer> contextSources = 
            new List<AssetReferenceContextContainer>();

        public List<AssetReferenceContextService> referenceServices = new List<AssetReferenceContextService>();

        public List<ContextService<IGameService>> services = new List<ContextService<IGameService>>();


        public async UniTask<IObservable<IContext>> Execute(ILifeTime lifeTime)
        {
            var source = await LoadDataSource(lifeTime);
            await ExecuteServices(source, lifeTime);
            return source;
        }

        public async UniTask<IObservable<IContext>> LoadDataSource(ILifeTime lifeTime)
        {
            var contextAssets = ClassPool.Spawn<List<ContextContainerAsset>>();
            
            if (contextSources?.Count <= 0) {
                GameLog.LogRuntime($"EMPTY context system sources");
                return Observable.Empty<IContext>();
            }
            
            //load all context sources
            await contextSources.LoadAssetsTaskAsync(contextAssets, lifeTime);
            //merge all source to single observable
            return contextAssets.Merge();
        }
        
        public async UniTask<Unit> ExecuteServices(IObservable<IContext> source,ILifeTime lifeTime)
        {
            var loadedServices = await referenceServices.
                    LoadScriptableAssetsTaskAsync<ContextService<IGameService>>(lifeTime);
            loadedServices.AddRange(services);

            await UniTask.WhenAll(
                loadedServices.Select(x => {
                    var task =x.Execute(source).Timeout(TimeSpan.FromSeconds(loadTimeout));
                    lifeTime.AddDispose(x);
                    return task;
                }));

            return Unit.Default;
        }
    }
}