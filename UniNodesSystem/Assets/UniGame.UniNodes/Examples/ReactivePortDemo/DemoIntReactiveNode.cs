using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Nodes;
using UniGreenModules.UniNodeSystem.Runtime.Core;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Examples.ReactivePortDemo
{
    using System;
    using NodeSystem.Runtime.Attributes;
    using NodeSystem.Runtime.ReactivePorts;
    using UnityEngine;

    [CreateNodeMenu("Examples/ReactivePortDemo/IntReactiveNode","IntReactivePorts")]
    public class DemoIntReactiveNode : UniNode
    {

        [ReactivePort()]
        [SerializeField]
        public IntReactivePort IntValueIn = new IntReactivePort();

        [ReactivePort(PortIO.Output)]
        [SerializeField]
        public IntReactivePort IntValueOut = new IntReactivePort();
        
    }
}
