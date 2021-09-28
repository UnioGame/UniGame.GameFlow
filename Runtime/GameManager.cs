namespace UniModules.UniGame.GameFlow.GameFlow.Runtime
{
    using System.Collections.Generic;
    using AddressableTools.Runtime.AssetReferencies;
    using AddressableTools.Runtime.Extensions;
    using Context.Runtime.Context;
    using Core.Runtime.DataFlow.Interfaces;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;
    using global::UniModules.GameFlow.Runtime.Core;
    using SerializableContext.Runtime.Addressables;
    using Taktika.GameRuntime.Abstract;
    using UniContextData.Runtime.Interfaces;
    using UniCore.Runtime.Rx.Extensions;
    using UnityEngine;

    public class GameManager : MonoBehaviour, IGameManager
    {
        #region inspector

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

        #endregion

        private EntityContext _gameContext = new EntityContext();

        #region public properties

        public static IGameManager Instance { get; protected set; }

        public IContext GameContext => _gameContext;

        public ILifeTime LifeTime => _gameContext.LifeTime;

        #endregion

        #region public methods

        public async UniTask Execute()
        {
            _gameContext.Cancel();
            _gameContext = new EntityContext();
            
            if (_contextContainer.RuntimeKeyIsValid())
            {
                var contextContainer = await _contextContainer.LoadAssetTaskAsync(LifeTime);
                contextContainer.SetValue(_gameContext);
            }

            await ExecuteSources(_gameContext);
            await ExecuteGraphs();
        }

        public void Dispose()
        {
            _gameContext?.Dispose();
            _gameContext = null;
        }

        #endregion

        #region private methods

        private UniTask ExecuteGraphs()
        {
            foreach (var graph in executionItems)
                graph.Execute();

            ExecuteAsyncGraphs()
                .AttachExternalCancellation(LifeTime.TokenSource)
                .Forget();
            
            return UniTask.CompletedTask;
        }

        private async UniTask ExecuteAsyncGraphs()
        {
            var asyncAsset = asyncGraphs.Select(asset => asset.LoadAssetTaskAsync(LifeTime));
            var graphs      = await UniTask.WhenAll(asyncAsset);
            foreach (var graphAsset in graphs)
            {
                var graphObject = Object.Instantiate(graphAsset.gameObject,transform);
                var graph       = graphObject.GetComponent<UniGraph>();
                graph.Execute();
                graph.AddTo(LifeTime);
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

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }
            
            Instance = this;
            foreach (var graph in executionItems)
                graph.AddTo(LifeTime);
            
            this.AddCleanUpAction(() => Instance = null);
        }

        private void OnDestroy() => Dispose();

        #endregion
    }
}
