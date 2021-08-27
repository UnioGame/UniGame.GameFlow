namespace UniGame.UniNodes.Nodes.Runtime.Logic
{
    using System.Collections.Generic;
    using Commands;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Core.Commands;
    using UniModules.UniGame.Core.Runtime.Interfaces;
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
