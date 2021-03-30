using System;
using System.Collections.Generic;
using UniGame.UniNodes.NodeSystem.Runtime.Attributes;
using UniGame.UniNodes.NodeSystem.Runtime.Core.Commands;
using UniGame.UniNodes.NodeSystem.Runtime.Core.Interfaces;
using UniGame.UniNodes.NodeSystem.Runtime.Core.Nodes;
using UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
using UniModules.UniGame.Core.Runtime.Interfaces;

namespace UniModules.UniGame.GameFlow.GameFlow.Runtime.Nodes.Common
{
    [Serializable]
    [HideNode]
    public class ProxyPortSNode : SNode
    {
        private static string DefaultPortName = "data";
        
        protected IPortValue inputPort;
        protected IPortValue outputPort;

        public IPortPair PortPair { get; private set; }
        
        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);
            
            var portCommand = new ConnectedFormatedPairCommand(this,DefaultPortName,true);
            nodeCommands.Add(portCommand);

            PortPair = portCommand;
            inputPort = portCommand.InputPort;
            outputPort = portCommand.OutputPort;
        }
        
        
    }
}
