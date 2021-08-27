namespace UniModules.UniGameFlow.GameFlowEditor.Editor.Extensions
{
    using System.Reflection;
    using global::UniModules.GameFlow.Runtime.Interfaces;
    using NodeSystem.Runtime.Extensions;

    public interface INodeHandler
    {
        PortField UpdatePortFieldData(INode node, FieldInfo fieldInfo);
    }
}