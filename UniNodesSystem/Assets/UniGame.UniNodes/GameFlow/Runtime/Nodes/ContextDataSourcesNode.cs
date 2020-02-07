using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Nodes.Runtime.Nodes;

namespace UniGreenModules.UniGameSystems.Runtime.Nodes
{
    using System.Collections.Generic;
    using Commands;
    using UniCore.Runtime.Interfaces;
    using UniGame.SerializableContext.Runtime.Addressables;
    using UniNodeSystem.Runtime.Core;
    using UniRx.Async;
    using UnityEngine;

    [CreateNodeMenu("GameSystem/Data Sources")]
    public class ContextDataSourcesNode : InOutPortNode
    {
        [Header("Node Output Data Source")]
        #if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity] 
        #endif
        public List<AsyncContextDataSourceAssetReference> sources;

        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);

            //create sync result for task
            var outputContextTask = UniTask.FromResult<IContext>(PortPair.OutputPort);
            //create node commands
            var sourceOutputPortCommand = new RegisterDataSourcesCommand<AsyncContextDataSourceAssetReference>(outputContextTask,sources);
            nodeCommands.Add(sourceOutputPortCommand);
        }
    }
}
