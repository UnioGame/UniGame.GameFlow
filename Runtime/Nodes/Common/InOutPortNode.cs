namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using System.Collections.Generic;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Core.Commands;
    using UniModules.GameFlow.Runtime.Core.Interfaces;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
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
