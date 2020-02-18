using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Attributes;
using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Nodes;
using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.ReactivePorts;
using UniGreenModules.UniNodeSystem.Runtime.Core;
using UnityEngine;

[CreateNodeMenu("Examples/SimpleCustomNodes/SimpleNode",nodeName = "Simple Node")]
public class SimpleDemoNode : UniNode
{

    [SerializeField]
    private int intDataOne;
    
    public int intDataTwo;

    [PortValue(PortIO.Input)]
    public int inInt;

    [PortValue(PortIO.Output)] 
    public float outInt;

    [ReactivePort()]
    public IntReactivePort inReactiveInt;

}
