namespace UniGame.UniNodes.GameFlow.Runtime.Nodes
{
    using System.Collections.Generic;
    using Commands;
    using Cysharp.Threading.Tasks;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGame.SerializableContext.Runtime.Addressables;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UniNodes.Nodes.Runtime.Common;
    
    using UnityEngine;

    [CreateNodeMenu("GameSystem/Data Sources")]
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
