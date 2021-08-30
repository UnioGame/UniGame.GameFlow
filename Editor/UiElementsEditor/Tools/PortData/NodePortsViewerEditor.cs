namespace UniModules.GameFlow.Editor.Tools.PortData
{
    using System;
    using System.Collections.Generic;
    using global::UniModules.GameFlow.Runtime.Interfaces;

    [Serializable]
    public class NodePortsViewerEditor
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineProperty]
        [Sirenix.OdinInspector.ListDrawerSettings(DraggableItems = false)]
#endif
        public List<PortViewerEditor> inputs  = new List<PortViewerEditor>();
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineProperty]
        [Sirenix.OdinInspector.ListDrawerSettings(DraggableItems = false)]
#endif
        public List<PortViewerEditor> outputs = new List<PortViewerEditor>();

        public void Initialize(INode node)
        {
            UpdatePorts(inputs,node.Inputs);
            UpdatePorts(outputs,node.Outputs);
        }

        private void UpdatePorts(List<PortViewerEditor> values, IEnumerable<INodePort> ports)
        {
            values.Clear();
            foreach (var port in ports)
            {
                values.Add(new PortViewerEditor().Initialize(port));
            }
        }
    }
}