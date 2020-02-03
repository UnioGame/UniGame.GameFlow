using UniGreenModules.UniNodeSystem.Runtime;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Nodes.Runtime.Nodes
{
    using System;
    using System.Collections.Generic;
    using NodeSystem.Runtime.Attributes;
    using NodeSystem.Runtime.Nodes;
    using UniCore.Runtime.Interfaces;
    using UniNodeSystem.Nodes.Commands;
    using UnityEngine;

    [HideNode]
    public class InOutPortNode : UniNode
    {
        private string defaultPortName = "data";

        #region inspector
        
        [SerializeField]
        private bool bindInOut = false;

        #endregion

        public IPortPair PortPair { get; private set; }

        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);

            var portCommand = new ConnectedFormatedPairCommand();
            portCommand.Initialize(this,defaultPortName,bindInOut);
            nodeCommands.Add(portCommand);

            PortPair = portCommand;
        }
    }
}
