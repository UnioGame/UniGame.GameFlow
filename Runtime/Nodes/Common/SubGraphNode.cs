namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Sirenix.OdinInspector;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Core.Nodes;
    using UniModules.GameFlow.Runtime.Extensions;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.AddressableTools.Runtime.AssetReferencies;
    using UniModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniModules.UniGame.Context.Runtime.Connections;
    using UniModules.UniGame.Context.Runtime.Context;
    using UniModules.UniGame.Core.Runtime.Extension;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGame.CoreModules.UniGame.Context.Runtime.Extension;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [Serializable]
    [CreateNodeMenu("SubGraph/SubGraphNode")]
    [NodeInfo(nameof(GraphContextOutputNode), "SubGraph", "create instance of subgraph and launch")]
    public class SubGraphNode : SNode
    {
        public const string dataIn  = "in";
        public const string dataOut = "out";
        
        public const string SubGraphNodeName = "SubGraph";
        
        #region inspector

        [HideInInspector]
        public string graphName;
        
        [DrawWithUnity]
        [HideLabel]
        public AssetReferenceComponent<UniGraph> subGraph;

        #endregion
        
        public sealed override string ItemName => graphName;
        
        protected sealed override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
#if UNITY_EDITOR
            graphName = subGraph.editorAsset ? subGraph.editorAsset.name : SubGraphNodeName;
#endif

            base.UpdateCommands(nodeCommands);
            this.UpdatePortValue(dataIn, PortIO.Input);
            this.UpdatePortValue(dataOut, PortIO.Output);
        }

        protected override async UniTask OnExecute()
        {
            var graphAsset = await subGraph.LoadAssetTaskAsync(LifeTime);
            var input      = GetPortValue(dataIn);
            var context    = await input.ReceiveFirstAsync<IContext>(LifeTime);

            var parent = graphAsset.Root;
            var graph = Object.Instantiate(graphAsset.gameObject,parent)
                .DestroyWith(LifeTime)
                .GetComponent<UniGraph>();

            var connection = new ContextConnection();
            connection.Connect(context).AddTo(LifeTime);
            
            await graph.ExecuteAsync(connection);
        }
    }
}