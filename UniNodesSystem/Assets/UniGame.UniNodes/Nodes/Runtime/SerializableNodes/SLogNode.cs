using UniGame.UniNodes.NodeSystem.Runtime.Core;

namespace UniGame.UniNodes.Nodes.Runtime.SerializableNodes
{
    using NodeSystem.Runtime.Attributes;
    using NodeSystem.Runtime.Core.Nodes;

    [CreateNodeMenu("SNodes/Debug/Log",nodeName = "Log")]
    public class SLogNode : SNode
    {
        public int SintValue;

        public float SfloatValue;

        [Port()]
        public int SintPort;
        
        [Port(PortIO.Output)]
        public int SintPortOut;
    }
}
