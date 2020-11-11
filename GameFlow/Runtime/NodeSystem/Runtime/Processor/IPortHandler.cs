namespace UniModules.UniGameFlow.GameFlowEditor.Editor.Extensions
{
    using global::UniGame.UniNodes.NodeSystem.Runtime.Interfaces;

    public interface IPortHandler
    {
        bool UpdatePortValue(INode node, INodePort port, object fieldValue);
    }
}