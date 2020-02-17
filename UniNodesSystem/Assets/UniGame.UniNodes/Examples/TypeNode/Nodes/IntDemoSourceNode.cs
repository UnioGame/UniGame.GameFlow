using UnityEngine;

namespace UniGreenModules.UniGameSystems.Examples.ServiceNode
{
    using System.Collections;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Attributes;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Nodes;
    using UniNodeSystem.Runtime.Core;
    using UniRoutine.Runtime;
    using UniRoutine.Runtime.Extension;

    [CreateNodeMenuAttribute("Examples/TypeNodes/IntDemoSource")]
    public class IntDemoSourceNode : UniNode
    {
        [PortValue(PortIO.Output)]
        public float floatOut;
        
        [PortValue(PortIO.Output)]
        public int intOut;

        public float delay = 1;
        
        protected override void OnExecute()
        {
            IntSourceProcess().
                Execute().
                AddTo(LifeTime);
        }

        private IEnumerator IntSourceProcess()
        {
            var intPort = GetPort(nameof(intOut));
            var floatPort = GetPort(nameof(floatOut));
            
            var wait = new WaitForSeconds(delay);
            while (isActiveAndEnabled) {
                intPort.Value.Publish(intOut);
                floatPort.Value.Publish(floatOut);
                yield return wait;
            }
        }
    }
}
