using UniModules.UniGame.CoreModules.UniGame.AddressableTools.Runtime.AssetReferencies;
using UniModules.UniGame.CoreModules.UniGame.AddressableTools.Runtime.Extensions;

namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using System;
    using Cysharp.Threading.Tasks;
    using Sirenix.OdinInspector;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Context.Runtime.Connections;
    using UniModules.UniGame.Core.Runtime.Extension;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [Serializable]
    [CreateNodeMenu("Common/Graph/NestedGraph")]
    [NodeInfo(nameof(GraphContextOutputNode), "NestedGraph", "create instance of subgraph and launch")]
    public class SubGraphNode : SContextNode
    {
        public const string SubGraphNodeName = "NestedGraph";
        
        #region inspector

        [HideInInspector]
        public string graphName;
        
        [DrawWithUnity]
        [HideLabel]
        public AssetReferenceComponent<UniGraph> subGraph;

        public bool awaitGraph = false;
        
        #endregion
        
        public sealed override string ItemName => graphName;

        protected override async UniTask OnContextActivate(IContext context)
        {
            var graphAsset = await subGraph.LoadAssetTaskAsync(LifeTime);

            var parent = GraphData.Root;
            var graph = Object.Instantiate(graphAsset.gameObject,parent)
                .DestroyWith(LifeTime)
                .GetComponent<UniGraph>();

            var connection = new ContextConnection();
            connection.Connect(context).AddTo(LifeTime);

            await LaunchGraph(graph, connection);
            
            Complete();
        }

        private async UniTask LaunchGraph(UniGraph graph, IDisposableContext context)
        {
            if (awaitGraph)
            {
                await graph.ExecuteAsync(context);
            }
            else
            {
                graph.ExecuteAsync(context)
                    .AttachExternalCancellation(LifeTime.TokenSource)
                    .Forget();
            }
        }
        
    }
}