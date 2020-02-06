using UnityEngine;

namespace UniGreenModules.UniGameSystems.Examples.ServiceNode
{
    using UniCore.Runtime.Rx.Extensions;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Nodes.Runtime.Nodes;
    using UniRx;

    [CreateNodeMenu("Examples/DemoSystem/SummatorNode")]
    public class SummatorDemoNode : TypeBridgeNode<int>
    {
        [SerializeField]
        private int summ = 0;

        protected override void OnExecute()
        {
            Source.
                Do(x => this.value += x).
                Do(x => Finish()).
                Subscribe().
                AddTo(LifeTime);
        }


    }
}
