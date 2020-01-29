using UnityEngine;

namespace UniGreenModules.UniGameSystems.Examples.ServiceNode
{
    using System.Collections;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Nodes.Runtime.Nodes;
    using UniNodeSystem.Runtime.Core;
    using UniRoutine.Runtime;
    using UniRoutine.Runtime.Extension;

    [CreateNodeMenuAttribute("Examples/DemoSystem/IntDemoSource")]
    public class IntDemoSourceNode : InOutPortNode
    {
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
            var wait = new WaitForSeconds(delay);
            while (isActiveAndEnabled) {
                PortPair.OutputPort.Publish(increment);
                yield return wait;
            }
        }
    }
}
