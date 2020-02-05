using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Nodes.Runtime.Nodes;

namespace UniGreenModules.UniGameSystems.Runtime.Nodes
{
    using System.Collections.Generic;
    using Commands;
    using UniContextData.Runtime.Interfaces;
    using UniCore.Runtime.Interfaces;
    using UniCore.Runtime.ObjectPool.Runtime;
    using UniCore.Runtime.ProfilerTools;
    using UniCore.Runtime.ReorderableInspector;
    using UniGame.AddressableTools.Runtime.Attributes;
    using UniGame.SerializableContext.Runtime.Addressables;
    using UniRx.Async;
    using UnityEngine;

    [CreateNodeMenu("GameSystem/Data Sources")]
    public class ContextDataSourcesNode : InOutPortNode
    {
        [Header("Node Output Data Source")]
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
