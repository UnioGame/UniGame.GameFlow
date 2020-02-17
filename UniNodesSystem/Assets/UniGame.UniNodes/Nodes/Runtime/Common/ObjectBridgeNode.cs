using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Nodes.Runtime.Nodes;
using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Attributes;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Nodes.Runtime.Common
{
    using NodeSystem.Runtime.Core.Commands;

    [HideNode]
    public class ObjectBridgeNode<T> : TypeBridgeNode<T>
        where T : class
    {
        
        [HideNodeInspector]
        public bool skipEmptyValue = true;

    }
}
