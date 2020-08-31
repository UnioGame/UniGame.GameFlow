namespace UniModules.UniGameFlow.GameFlowEditor.Editor.Extensions
{
    using System.Reflection;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
    using NodeSystem.Runtime.Extensions;

    public interface INodeHandler
    {
        PortField UpdatePortFieldData(INode node, FieldInfo fieldInfo);
    }
}