using System;
using UniModules.GameFlow.Runtime.Core.Nodes;

namespace UniModules.UniGameFlow.Nodes.Runtime.States
{
    using global::UniModules.GameFlow.Runtime.Attributes;
    using global::UniModules.GameFlow.Runtime.Core;

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
