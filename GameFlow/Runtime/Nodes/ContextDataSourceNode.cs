namespace UniGame.UniNodes.GameFlow.Runtime.Nodes
{
    using System.Collections.Generic;
    using Commands;
    using NodeSystem.Runtime.Core;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniGame.SerializableContext.Runtime.Addressables;
    using UniNodes.Nodes.Runtime.Common;
    using UniRx.Async;

    [CreateNodeMenu("GameSystem/Data Source")]
    public class ContextDataSourceNode : InOutPortNode
    {
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
