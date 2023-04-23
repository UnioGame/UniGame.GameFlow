using UniGame.AddressableTools.Runtime;

namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using System;
    using Cysharp.Threading.Tasks;
    using Sirenix.OdinInspector;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.UniGame.Context.Runtime.Connections;
    using Core.Runtime;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [Serializable]
    [CreateNodeMenu("Common/Graph/NestedGraphNode")]
    [NodeInfo(nameof(GraphContextOutputNode), 
        "NestedGraph", 
        "create instance of subgraph and launch")]
    public class NestedGraphNode : SContextNode
    {
        public const string NestedGraphNodeName = "NestedGraph";
        
        #region inspector

        [HideInInspector]
        public string graphName;
        
        [DrawWithUnity]
        [HideLabel]
        public AssetReferenceComponent<UniGraph> nestedGraph;

        public bool awaitGraph = false;
        
        #endregion

        public sealed override string ItemName
        {
            get
            {
#if UNITY_EDITOR
                if (nestedGraph?.editorAsset != null)
                    return $"[{nestedGraph.editorAsset.gameObject.name}]";
#endif
                return string.IsNullOrEmpty(graphName) 
                    ? NestedGraphNodeName
                    : graphName;
            }
        }

        protected override async UniTask<bool> OnContextActivate(IContext context)
        {
            var graphAsset = await nestedGraph.LoadAssetTaskAsync(LifeTime);
            var graphGameObject = graphAsset.gameObject;
            var parent = GraphData.Root;
            var graph = Object
                .Instantiate(graphGameObject,parent)
                .DestroyWith(LifeTime)
                .GetComponent<UniGraph>();

            graphName = graphGameObject.name;
            
            var connection = new ContextConnection();
            connection.Connect(context)
                .AddTo(LifeTime);

            await LaunchGraph(graph, connection);

            return true;
        }

        private async UniTask LaunchGraph(UniGraph graph, IContextConnection context)
        {
            if (awaitGraph)
            {
                await graph.ExecuteAsync(context);
                return;
            }
            
            graph.ExecuteAsync(context)
                .AttachExternalCancellation(LifeTime.CancellationToken)
                .Forget();
        }
        
    }
}