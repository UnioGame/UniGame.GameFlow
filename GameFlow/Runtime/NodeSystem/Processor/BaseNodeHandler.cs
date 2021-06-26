namespace UniModules.UniGameFlow.GameFlowEditor.Editor.Processor
{
    using System;
    using System.Reflection;
    using Extensions;
    using global::UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor.Extensions;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
    using NodeSystem.Runtime.Extensions;

    [Serializable]
    public class 
        BaseNodeHandler : INodeHandler
    {
        public PortField UpdatePortFieldData(INode node, FieldInfo fieldInfo)
        {
            var portData = node.GetPortData(fieldInfo);
            return portData;
        }
    }
}