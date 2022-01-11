using System;
using UniModules.GameFlow.Runtime.Core.Nodes;
using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;

namespace UniModules.UniGameFlow.Nodes.Runtime.States
{
    using Cysharp.Threading.Tasks;
    using global::UniModules.GameFlow.Runtime.Attributes;
    using global::UniModules.GameFlow.Runtime.Core;

    [Serializable]
    [CreateNodeMenu("Common/States/StateTokenNode")]
    public class StateTokenNode : SNode
    {
        [Port]
        public object input;
        
        [Port(PortIO.Output)]
        public object output;

        protected override UniTask OnExecute()
        {
            var inputPort  = GetPort(nameof(input));
            var outputPort = GetPort(nameof(output));
            
            return UniTask.CompletedTask;
        }
    }
}
