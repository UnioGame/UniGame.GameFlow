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

        [PortValue(PortIO.Output)]
        public int intOut1;

        
        public int interval = 1;
        public float delay = 1;
        
        protected override void OnExecute()
        {
            IntSourceProcess(interval, delay).
                Execute().
                AddTo(LifeTime);
        }

        private IEnumerator IntSourceProcess(int increment, float updateDelay)
        {
            var intPort = GetPort(nameof(intOut));
            var floatPort = GetPort(nameof(floatOut));
            
            var wait = new WaitForSeconds(updateDelay);
            while (isActiveAndEnabled) {
                intPort.Publish(increment);
                floatPort.Publish(Time.deltaTime);
                yield return wait;
            }
        }
    }
}
