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


    [CreateNodeMenu("Common/Sources/Context Data Source")]
    public class ContextDataSourceNode : InOutPortNode
    {
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        public AssetReferenceDataSource contextDataSource;

        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);

            //create sync result for task
            var outputContextTarget = UniTask.FromResult<IContext>(PortPair.OutputPort);
            //create node commands
            nodeCommands.Add(new RegisterDataSourceCommand(outputContextTarget,contextDataSource));
        }

    }
}
