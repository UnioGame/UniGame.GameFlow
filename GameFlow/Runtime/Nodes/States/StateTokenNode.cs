using System;
using UniGame.UniNodes.NodeSystem.Runtime.Core.Nodes;

namespace UniModules.UniGameFlow.Nodes.Runtime.States
{
    using global::UniGame.UniNodes.NodeSystem.Runtime.Attributes;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Core;

    [Serializable]
    public class StateTokenNode : SNode
    {
        [Port]
        public object input;
        
        [Port(PortIO.Output)]
        public object output;

        protected override void OnExecute()
        {
            var inputPort  = GetPort(nameof(input));
            var outputPort = GetPort(nameof(output));
            
        }
    }
}
