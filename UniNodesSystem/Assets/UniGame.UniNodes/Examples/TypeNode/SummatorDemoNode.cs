using UnityEngine;

namespace UniGreenModules.UniGameSystems.Examples.ServiceNode
{
    using UniCore.Runtime.Interfaces;
    using UniCore.Runtime.ProfilerTools;
    using UniFlowNodes;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Nodes.Runtime.Nodes;

    [CreateNodeMenu("Examples/DemoSystem/SummatorNode")]
    public class SummatorDemoNode : TypeBridgeNode<int>
    {
        [SerializeField]
        private int summ = 0;
        
        protected override void OnDataUpdated(int data, IContext source, IContext target)
        {

            summ += data;
            GameLog.Log($"{graph.name}:{name} SUMM = {summ}");
            
            target.Publish(summ);

        }
    }
}
