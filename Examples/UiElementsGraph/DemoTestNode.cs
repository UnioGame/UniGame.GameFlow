using UniGame.UniNodes.NodeSystem.Runtime.Attributes;
using UniGame.UniNodes.NodeSystem.Runtime.Core;
using UniGame.UniNodes.NodeSystem.Runtime.Core.Nodes;
using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;

[CreateNodeMenu("Demo/DemoTestNode")]
public class DemoTestNode : SNode
{
    [Port(PortIO.Input)]
    public float inFloat;
    
    [Port(PortIO.Output)]
    public float outFloat;

}
