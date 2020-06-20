namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using Runtime.Interfaces;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;

    [CreateNodeMenu("Common/EndPoint","EndPoint")]
    public class UniCancelationNode : UniPortNode, IGraphCancelationNode
    {
        
    }
    
}
