namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Nodes.Runtime.Logic
{
    using System.Collections.Generic;
    using NodeSystem.Runtime.Nodes;
    using UniCore.Runtime.Interfaces;
    using UniGreenModules.UniNodes.Runtime.Commands;
    using UniNodeSystem.Nodes.Commands;
    using UniNodeSystem.Runtime.Core;

    [CreateNodeMenu("Common/DataDelay")]
    public class DataDelayNode : UniNode
    {
        #region inspector

        public float delay;
        
        #endregion
        
        private const string Input = "Value";
        
        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            //make in/out ports
            var portCommand = new ConnectedFormatedPairCommand(this,Input,false);
            nodeCommands.Add(portCommand);

            //register data delay command
            var delayCommand = new PortValueTransferDelayCommand(portCommand.InputPort, portCommand.OutputPort, delay);
            nodeCommands.Add(delayCommand);
            
        }
    }
}
