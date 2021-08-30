namespace UniModules.UniGameFlow.GameFlowEditor.Editor.Extensions
{
    using global::UniModules.GameFlow.Runtime.Interfaces;

    public interface IPortHandler
    {
        bool UpdatePortValue(INode node, INodePort port, object fieldValue);
    }
}