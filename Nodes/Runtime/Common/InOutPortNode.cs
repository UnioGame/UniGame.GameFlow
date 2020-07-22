namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using System.Collections.Generic;
    using NodeSystem.Runtime.Attributes;
    using NodeSystem.Runtime.Core;
    using NodeSystem.Runtime.Core.Commands;
    using NodeSystem.Runtime.Core.Interfaces;
    using NodeSystem.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UnityEngine;

    [HideNode]
    public class InOutPortNode : UniNode
    {
        private string defaultPortName = "data";

        #region inspector
        
        [SerializeField]
        private bool bindInOut = false;

        protected IPortValue inputPort;
        protected IPortValue outputPort;
        
        #endregion

        public IPortPair PortPair { get; private set; }

        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);

            var portCommand = new ConnectedFormatedPairCommand(this,defaultPortName,bindInOut);
            nodeCommands.Add(portCommand);

            PortPair = portCommand;
            inputPort = portCommand.InputPort;
            outputPort = portCommand.OutputPort;
        }
    }
}
