using UniGame.UniNodes.NodeSystem.Runtime.Attributes;
using UniGame.UniNodes.NodeSystem.Runtime.Core;
using UniGame.UniNodes.NodeSystem.Runtime.Core.Nodes;

[CreateNodeMenu("Demo/DemoTestNode")]
public class DemoTestNode : SNode
{
    [Port(PortIO.Input)]
    public float inFloat;
}
