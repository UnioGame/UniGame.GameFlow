namespace UniGame.UniNodes.GameFlow.Runtime.Nodes
{
    using System.Collections.Generic;
    using Commands;
    using NodeSystem.Runtime.Core;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniGame.AddressableTools.Runtime.Attributes;
    using UniGreenModules.UniGame.Context.Runtime.Interfaces;
    using UniGreenModules.UniGame.SerializableContext.Runtime.Addressables;
    using UniNodes.Nodes.Runtime.Common;
    using UniRx.Async;
    using UnityEngine;

    [CreateNodeMenu("GameSystem/Data Source")]
    public class ContextDataSourceNode : InOutPortNode
    {
        [ShowAssetReference]
        [Header("Node Output Data Source")]
        public AsyncContextDataSourceAssetReference contextDataSource;

        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);

            //create sync result for task
            var outputContextTask = UniTask.FromResult<IContext>(PortPair.OutputPort);
            //create node commands
            var sourceOutputPortCommand = new 
                RegisterDataSourceCommand(outputContextTask,contextDataSource);
            
            nodeCommands.Add(sourceOutputPortCommand);
        }

        protected sealed override void OnExecute()
        {
            LifeTime.AddDispose(contextDataSource);
        }
    }
}
