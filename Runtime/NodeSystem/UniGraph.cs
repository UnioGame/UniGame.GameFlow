using UniGame.GameFlow;
using UniGame.GameFlowEditor.Runtime;
using UniGame.AddressableTools.Runtime;
using UniModules.UniGame.Context.Runtime.Connections;
using UniGame.Core.Runtime.Extension;

namespace UniModules.GameFlow.Runtime.Core
{
    using System.Collections.Generic;
    using Attributes;
    using Cysharp.Threading.Tasks;
    using Runtime.Extensions;
    using Runtime.Interfaces;
    using global::UniGame.Context.Runtime;
    using UnityEngine;

    [HideNode]
    public class UniGraph : NodeGraph, IUniGraph
    {
        #region inspector properties
        
        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        private List<AssetReferenceDataSource> _asyncDataSources = new List<AssetReferenceDataSource>();
        
        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        private List<AsyncSource> _dataSources = new List<AsyncSource>();

        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineEditor()]
#endif
        public UniGraphAsset serializedGraph;

        #if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineProperty]
        #endif
        [SerializeReference]
        public IUniGraphProcessor graphProcessor = new UniGraphProcessor();

        #endregion
        
        #region private properties
        
        /// <summary>
        /// graph context
        /// </summary>
        private IContextConnection _graphContext = new ContextConnection();

        /// <summary>
        /// graph inputs
        /// </summary>
        private List<IGraphPortNode> inputs = new List<IGraphPortNode>();
        
        /// <summary>
        /// graph outputs
        /// </summary>
        private List<IGraphPortNode> outputs = new List<IGraphPortNode>();

        #endregion

        public sealed override IContextConnection GraphContext => _graphContext;

        public GameObject AssetInstance => gameObject;

        public IReadOnlyList<IGraphPortNode> OutputsPorts => outputs;
        
        public IReadOnlyList<IGraphPortNode> InputsPorts => inputs;

        public void Initialize()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged -= OnPlayingModeChanged;
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayingModeChanged;
#endif
            
            Initialize(this);
        }

        public async UniTask ExecuteAsync(IContextConnection context)
        {
            _graphContext = context;
            await ExecuteAsync();
        }
        
        #region private methods

        protected sealed override IEnumerable<INode> GetCustomNodes()
        {
            if(serializedGraph == null) 
                yield break;

            yield break;

            var items = NodeDataConverter.ConvertNodes(this,serializedGraph);

            foreach (var item in items)
                yield return item;
        }

        protected sealed override void OnInitialize()
        {
            base.OnInitialize();
            
            InitializeGraphNodes();
#if UNITY_EDITOR
            if (Application.isPlaying == false) 
                Validate();
#endif

        }

        protected sealed override UniTask OnExecute()
        {
            LifeTime.AddDispose(_graphContext);
            LifeTime.AddCleanUpAction(() => _graphContext = new ContextConnection());
            
            graphProcessor?.ExecuteAsync(this)
                .AttachExternalCancellation(LifeTime.Token)
                .Forget();

            LoadDataSources()
                .AttachExternalCancellation(LifeTime.Token)
                .Forget();
            
            return UniTask.CompletedTask;
        }
        
        private async UniTask LoadDataSources()
        {
            UniTask.WhenAll(_dataSources.Select(x => x
                    .ToSharedInstance(LifeTime)
                    .RegisterAsync(Context)))
                .AttachExternalCancellation(LifeTime.Token)
                .Forget();

            foreach (var referenceSource in _asyncDataSources) {
                var source = await referenceSource
                    .LoadAssetTaskAsync(LifeTime)
                    .ToSharedInstanceAsync(LifeTime);
                
                if (source is not IAsyncDataSource asyncSource) continue;
                
                asyncSource.RegisterAsync(Context)
                    .AttachExternalCancellation(LifeTime.Token)
                    .Forget();
            }
        }

        private void InitializeGraphNodes()
        {
            inputs.Clear();
            outputs.Clear();
            
            for (var i = 0; i < Nodes.Count; i++) {

                var node = Nodes[i];
                
                //register graph ports by nodes
                UpdatePortNode(node);
                
                //initialize node
                node.Initialize(this);
                
                //update ports by attributes & another triggers
                node.UpdateNodePorts();
            }
            
        }
        
        private void UpdatePortNode(INode uniNode)
        {
            //register input/output nodes
            if (!(uniNode is IGraphPortNode graphPortNode)) {
                return;
            }

            var container = graphPortNode.Direction == PortIO.Input 
                ? inputs 
                : outputs;
      
            //add graph ports for exists port nodes
            this.UpdatePortValue(graphPortNode.ItemName, graphPortNode.Direction);
               
            container.Add(graphPortNode);

        }

        private void Awake() => _graphContext = new ContextConnection();
        
        #endregion
        
        #region editor api
        
#if UNITY_EDITOR

        private void ReleaseNodes()
        {
            if (!this) return;
            
            Release();
            Nodes.ForEach(x =>
            {
                if(x is IUniNode uniNode) uniNode.Release();
            });
        }

        private void OnPlayingModeChanged(UnityEditor.PlayModeStateChange mode)
        {
            UnityEditor.EditorApplication.playModeStateChanged -= OnPlayingModeChanged;
            ReleaseNodes();
        }

#endif

        #endregion
    }
}