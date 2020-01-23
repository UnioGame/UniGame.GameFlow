namespace UniGame.Nodes.Logic
{
    using System.Collections.Generic;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniNodes.Runtime.Commands;
    using UniGreenModules.UniNodeSystem.Nodes.Commands;
    using UniGreenModules.UniNodeSystem.Runtime;

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
            var portCommand = new ConnectedFormatedPairCommand();
            portCommand.Initialize(this,Input,false);
            nodeCommands.Add(portCommand);

            //register data delay command
            var delayCommand = new PortValueTransferDelayCommand(portCommand.InputPort, portCommand.OutputPort, delay);
            nodeCommands.Add(delayCommand);
            
        }
    }
}
