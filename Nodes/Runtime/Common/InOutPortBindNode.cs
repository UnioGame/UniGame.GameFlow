namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using System.Collections.Generic;
    using NodeSystem.Runtime.Attributes;
    using NodeSystem.Runtime.Core;
    using NodeSystem.Runtime.Core.Commands;
    using NodeSystem.Runtime.Core.Interfaces;
    using UniModules.UniCore.Runtime.Interfaces;

    [HideNode]
    public class InOutPortBindNode : UniNode
    {
        private const string defaultPortName = "data";
        
        public IPortPair PortPair { get; private set; }

        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);

            var portCommand = new ConnectedFormatedPairCommand(this,defaultPortName,true);
            nodeCommands.Add(portCommand);

            PortPair = portCommand;
        }
    }
}
