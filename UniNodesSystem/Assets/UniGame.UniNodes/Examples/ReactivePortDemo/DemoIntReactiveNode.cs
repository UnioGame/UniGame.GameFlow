using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Nodes;
using UniGreenModules.UniNodeSystem.Runtime.Core;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Examples.ReactivePortDemo
{
    using NodeSystem.Runtime.ReactivePorts;
    using UnityEngine;

    [CreateNodeMenu("Examples/ReactivePortDemo/IntReactiveNode","IntReactivePorts")]
    public class DemoIntReactiveNode : UniNode
    {

        [SerializeField]
        public IntReactivePort IntValue1 = new IntReactivePort();
        
    }
}
