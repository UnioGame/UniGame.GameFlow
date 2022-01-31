using UniModules.UniGame.AddressableTools.Runtime.AssetReferencies;
using UniModules.UniGame.AddressableTools.Runtime.Extensions;
using UniModules.UniGame.SerializableContext.Runtime.Addressables;

namespace UniModules.UniGame.GameFlow.GameFlow.Runtime
{
    using System.Collections.Generic;
    using Context.Runtime.Context;
    using Core.Runtime.DataFlow.Interfaces;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;
    using global::UniModules.GameFlow.Runtime.Core;
    using SerializableContext.Runtime.Addressables;
    using Taktika.GameRuntime.Abstract;
    using UniContextData.Runtime.Interfaces;
    using UniCore.Runtime.DataFlow;
    using UniCore.Runtime.Rx.Extensions;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class GameManager : MonoBehaviour, IGameManager
    {
        #region inspector

        public bool isEnabled = true;
        
        [SerializeField]
        private AssetReferenceContextContainer _contextContainer;

        [SerializeField]
        private List<UniGraph> executionItems = new List<UniGraph>();

        [SerializeField]
        private List<AssetReferenceComponent<UniGraph>> asyncGraphs = new List<AssetReferenceComponent<UniGraph>>();
        
        [SerializeField]
        private List<AssetReferenceDataSource> _assetSources = new List<AssetReferenceDataSource>();

        [SerializeReference]
        private List<IAsyncContextDataSource> _sources = new List<IAsyncContextDataSource>();

        [SerializeReference]
        private bool executeOnStart = true;
        
        #endregion

        private LifeTimeDefinition _lifeTime = new LifeTimeDefinition();
        
        private EntityContext _gameContext = new EntityContext();

        #region public properties

        public IContext GameContext => _gameContext;

        public ILifeTime LifeTime => _lifeTime;

        #endregion

        #region public methods

        public async UniTask Execute()
        {
            if (!isEnabled) return;
            
            _gameContext = new EntityContext().AddTo(LifeTime);
            
            if (_contextContainer.RuntimeKeyIsValid())
            {
                var contextContainer = await _contextContainer.LoadAssetTaskAsync(LifeTime);
                contextContainer.SetValue(_gameContext);
            }

            await ExecuteSources(_gameContext);
            await ExecuteGraphs();
        }

        public void Destroy()
        {
            Dispose();
            Object.Destroy(gameObject);
        }
        
        public void Dispose() => _lifeTime.Terminate();

        #endregion

        #region private methods

        private UniTask ExecuteGraphs()
        {
            foreach (var graph in executionItems)
                graph.AddTo(LifeTime)
                    .ExecuteAsync()
                    .AttachExternalCancellation(LifeTime.TokenSource)
                    .Forget();

            ExecuteAsyncGraphs(_gameContext)
                .AttachExternalCancellation(LifeTime.TokenSource)
                .Forget();
            
            return UniTask.CompletedTask;
        }

        private async UniTask ExecuteAsyncGraphs(IContext context)
        {
            var asyncAsset = asyncGraphs.Select(asset => asset.LoadAssetTaskAsync(LifeTime));
            var graphs      = await UniTask.WhenAll(asyncAsset);
            foreach (var graphAsset in graphs)
            {
                var graphObject = Object.Instantiate(graphAsset.gameObject,transform);
                var graph       = graphObject.GetComponent<UniGraph>();
                graph.AddTo(LifeTime)
                    .ExecuteAsync()
                    .AttachExternalCancellation(LifeTime.TokenSource)
                    .Forget();
            }
        }

        private UniTask ExecuteSources(IContext context)
        {
            foreach (var source in _sources)
                source.RegisterAsync(context)
                    .AttachExternalCancellation(LifeTime.TokenSource)
                    .Forget();
            
            foreach (var sourceReference in _assetSources)
                RegisterSource(sourceReference, context)
                    .AttachExternalCancellation(LifeTime.TokenSource)
                    .Forget();
            
            return UniTask.CompletedTask;
        }

        private async UniTask RegisterSource(AssetReferenceDataSource dataSource, IContext context)
        {
            var sourceAsset = await dataSource.LoadAssetTaskAsync(LifeTime);
            await sourceAsset.RegisterAsync(context);
        }

        private void OnDestroy() => Dispose();

        #endregion
    }
}
