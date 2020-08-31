using UniGame.UniNodes.NodeSystem.Runtime.Core.Nodes;

namespace UniModules.UniGameFlow.Nodes.Runtime.States
{
    using System;
    using System.Collections.Generic;
    using global::UniGame.UniNodes.Nodes.Runtime.Common;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Attributes;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;

    [Serializable]
    public class StateToken
    {
        public IPortValue PreviousState;
    }
    
    [HideNode]
    [Serializable]
    public class StateNode : SContextNode
    {
        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);
            
            //add commands to translate state token to output ports
        }
        
        
    }
}
