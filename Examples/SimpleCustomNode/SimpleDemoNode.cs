using UnityEngine;

namespace UniGame.UniNodes.Examples.SimpleCustomNode
{
    using NodeSystem.Runtime.Attributes;
    using NodeSystem.Runtime.Core;
    using NodeSystem.Runtime.ReactivePorts;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;

    [CreateNodeMenu("Examples/SimpleCustomNodes/SimpleNode",nodeName = "Simple Node")]
    public class SimpleDemoNode : UniNode
    {

        [SerializeField]
        private int intDataOne;
    
        public int intDataTwo;

        [Port(PortIO.Input)]
        public int inInt;

        [Port(PortIO.Output)] 
        public float outInt;

        [ReactivePort()]
        public IntReactivePort inReactiveInt;

    }
}
