namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using System.Collections.Generic;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Core.Commands;
    using UniModules.GameFlow.Runtime.Core.Interfaces;
    using Core.Runtime;

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
