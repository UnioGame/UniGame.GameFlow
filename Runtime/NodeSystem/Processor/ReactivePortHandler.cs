namespace UniModules.UniGameFlow.GameFlowEditor.Editor.Processor
{
    using System;
    using Extensions;
    using global::UniModules.GameFlow.Runtime.Core.Interfaces;
    using global::UniModules.GameFlow.Runtime.Interfaces;

    [Serializable]
    public class ReactivePortHandler : IPortHandler
    {
        public bool UpdatePortValue(INode node, INodePort port, object fieldValue)
        {
            if (fieldValue is IReactiveSource reactiveSource) {
                reactiveSource.Bind(node,port.ItemName);
                return true;
            }

            return false;
        }
        
    }
}