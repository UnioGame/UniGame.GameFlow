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
    using Object = UnityEngine.Object;
    using Quaternion = UnityEngine.Quaternion;
    using Vector3 = UnityEngine.Vector3;

    [Serializable]
    [CreateNodeMenu("SubGraph/SubGraphNode")]
    [NodeInfo(nameof(GraphContextOutputNode), "SubGraph", "create instance of subgraph and launch")]
    public class SubGraphNode : SNode
    {
        public const string dataIn  = "in";
        public const string dataOut = "out";
        
        public const string SubGraphNodeName = "SubGraph";
        
        #region inspector

        [DrawWithUnity]
        public AssetReferenceComponent<UniGraph> subGraph;

        public bool passContext = true;
        
        #endregion
        
        public sealed override string ItemName => SubGraphNodeName;
        
        protected sealed override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
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

            IDisposableContext graphContext = null;
            
            if (passContext)
            {
                var connection = new ContextConnection();
                connection.Connect(context).AddTo(LifeTime);
                graphContext = connection;
            }

            graphContext ??= new EntityContext();
            
            await graph.ExecuteAsync(graphContext);
        }
    }
}