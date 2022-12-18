using System;
using UniGame.GameRuntime.Abstract;
using UniModules.GameFlow.Runtime.Interfaces;
using UniModules.UniGame.Context.Runtime.Connections;

namespace UniModules.UniGame.GameFlow.GameFlow.Runtime
{
    using global::UniGame.AddressableTools.Runtime;
    using System.Collections.Generic;
    using Context.Runtime.Context;
    using global::UniGame.Core.Runtime;
    using Cysharp.Threading.Tasks;
    using global::UniModules.GameFlow.Runtime.Core;
    using global::UniGame.Context.Runtime;
    using UniCore.Runtime.DataFlow;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class GameManager : MonoBehaviour, IGameManager
    {
        #region inspector

        public bool isEnabled = true;

        [SerializeField]
        public AssetReferenceContextContainer contextContainer;

        [SerializeField]
        public List<UniGraph> gameFlows = new List<UniGraph>();

        [SerializeField]
        public List<AssetReferenceGameFlow> asyncGraphs = new List<AssetReferenceGameFlow>();
        
        [SerializeField]
        public List<AssetReferenceDataSource> asyncDataSources = new List<AssetReferenceDataSource>();

        [SerializeReference]
        public List<IAsyncDataSource> dataSources = new List<IAsyncDataSource>();

        [SerializeReference]
        public bool executeOnStart = true;
        
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

            _gameContext = new EntityContext();

            if (contextContainer.RuntimeKeyIsValid())
            {
                var container = await contextContainer.LoadAssetTaskAsync(LifeTime);
                container.SetValue(_gameContext);
            }

            await ExecuteSources(_gameContext);
            await ExecuteGraphs(_gameContext);
        }

        public void Destroy()
        {
            Dispose();
            Object.Destroy(gameObject);
        }
        
        public void Dispose() => _lifeTime.Terminate();

        #endregion

        #region private methods

        private UniTask ExecuteGraphs(IContext context)
        {
            foreach (var graph in gameFlows)
            {
                ExecuteGameFlowAsync(graph,context)
                    .AttachExternalCancellation(LifeTime.TokenSource)
                    .Forget();
            }

            ExecuteAsyncFlows(_gameContext)
                .AttachExternalCancellation(LifeTime.TokenSource)
                .Forget();
            
            return UniTask.CompletedTask;
        }

        private async UniTask ExecuteAsyncFlows(IContext context)
        {
            var asyncAsset = asyncGraphs
                .Select(asset => asset.LoadAssetTaskAsync(LifeTime));
            
            var graphs      = await UniTask.WhenAll(asyncAsset);
            foreach (var graphAsset in graphs)
            {
                var graphObject = Instantiate(graphAsset.gameObject,transform);
                var graph       = graphObject.GetComponent<UniGraph>();
                
                ExecuteGameFlowAsync(graph,context)
                    .AttachExternalCancellation(LifeTime.TokenSource)
                    .Forget();
            }
        }

        private async UniTask ExecuteGameFlowAsync(IUniGraph graph,IContext context)
        {
            var connection = new ContextConnection();
            connection.Connect(context).AddTo(LifeTime);
            await graph.AddTo(LifeTime).ExecuteAsync(connection);
        }

        private UniTask ExecuteSources(IContext context)
        {
            foreach (var source in dataSources)
                source.RegisterAsync(context)
                    .AttachExternalCancellation(LifeTime.TokenSource)
                    .Forget();
            
            foreach (var sourceReference in asyncDataSources)
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

    [Serializable]
    public class AssetReferenceGameFlow : AssetReferenceComponent<UniGraph>
    {
        public AssetReferenceGameFlow(string guid) : base(guid)
        {
        }
    }
}
