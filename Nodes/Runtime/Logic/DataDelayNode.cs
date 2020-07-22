namespace UniGame.UniNodes.Nodes.Runtime.Logic
{
    using System.Collections.Generic;
    using Commands;
    using NodeSystem.Runtime.Core;
    using NodeSystem.Runtime.Core.Commands;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;

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
