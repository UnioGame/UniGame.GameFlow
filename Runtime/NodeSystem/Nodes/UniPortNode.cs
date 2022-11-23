namespace UniModules.GameFlow.Runtime.Core
{
    using System.Collections.Generic;
    using Commands;
    using Runtime.Interfaces;
    using global::UniGame.Core.Runtime;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;

    [CreateNodeMenu("Common/PortNode","GraphPort")]
    public class UniPortNode : UniNode, IUniPortNode
    {
                
#region inspector
        
        public PortIO direction = PortIO.Input;

        public bool bindInOut = true;
        
#endregion

#region private properties

        private ConnectedFormatedPairCommand portPairCommand;

#endregion

        public PortIO Direction => direction;

        public IPortValue PortValue { get; protected set; }

        public IPortValue Input => portPairCommand.InputPort;

        public IPortValue Output => portPairCommand.OutputPort;

        public bool Visible => false;

        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);
            
            portPairCommand =  new ConnectedFormatedPairCommand(this, "data", bindInOut);
            PortValue = Direction == PortIO.Input ? 
                portPairCommand.InputPort : 
                portPairCommand.OutputPort;
            
            nodeCommands.Add(portPairCommand);
        }
    }
        
}
