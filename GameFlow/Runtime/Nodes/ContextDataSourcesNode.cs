namespace UniGame.UniNodes.GameFlow.Runtime.Nodes
{
    using System.Collections.Generic;
    using Commands;
    using NodeSystem.Runtime.Core;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniGame.SerializableContext.Runtime.Addressables;
    using UniNodes.Nodes.Runtime.Common;
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

        protected sealed override void OnExecute()
        {
            foreach (var reference in sources) {
                LifeTime.AddDispose(reference);
            }
        }
    }
}
