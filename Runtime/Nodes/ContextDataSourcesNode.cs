using UniModules.GameFlow.Runtime.Attributes;

namespace UniGame.UniNodes.GameFlow.Runtime.Nodes
{
    using System.Collections.Generic;
    using Commands;
    using Cysharp.Threading.Tasks;
    using UniModules.GameFlow.Runtime.Core;
    using Core.Runtime;
    using Context.Runtime;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UniNodes.Nodes.Runtime.Common;
    
    using UnityEngine;

    [CreateNodeMenu("Common/Sources/Data Sources")]
    [NodeInfo(category:"Sources", description:"Register ALL sources into output port")]
    public class ContextDataSourcesNode : InOutPortNode
    {
        [Header("Node Output Data Source")]
        #if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity] 
        #endif
        public List<AssetReferenceDataSource> sources;

        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);

            //create sync result for task
            var outputContextTask = UniTask.FromResult<IContext>(PortPair.OutputPort);
            //create node commands
            var sourceOutputPortCommand = new RegisterDataSourcesCommand<AssetReferenceDataSource>(outputContextTask,sources);
            nodeCommands.Add(sourceOutputPortCommand);
        }
    }
}
