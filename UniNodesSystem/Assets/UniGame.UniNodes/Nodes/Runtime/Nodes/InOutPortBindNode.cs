namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Nodes.Runtime.Nodes
{
    using System.Collections.Generic;
    using NodeSystem.Runtime.Attributes;
    using NodeSystem.Runtime.Nodes;
    using UniCore.Runtime.Interfaces;
    using UniNodeSystem.Nodes.Commands;

    [HideNode]
    public class InOutPortBindNode : UniNode
    {
        private const string defaultPortName = "data";
        
        public IPortPair PortPair { get; private set; }

        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);

            var portCommand = new ConnectedFormatedPairCommand();
            portCommand.Initialize(this,defaultPortName,true);
            nodeCommands.Add(portCommand);

            PortPair = portCommand;
        }
    }
}
