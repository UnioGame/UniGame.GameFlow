namespace UniGreenModules.UniNodes.Runtime.Nodes
{
    using System;
    using UniCore.Runtime.Interfaces;
    using UniFlowNodes;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Nodes.Runtime.Nodes;

    [Serializable]
    [CreateNodeMenu("Common/ContextNode")]
    public class ContextNode : TypeBridgeNode<IContext>
    {
        
    }
}
