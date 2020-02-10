namespace UniGreenModules.UniNodeSystem.Inspector.Editor.Styles
{
    using BaseEditor;
    using Runtime;
    using Runtime.Core;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Nodes;
    using UnityEngine;

    public class PortStyleSelector : IPortStyleProvider
    {
    
        public virtual NodeGuiLayoutStyle Select(NodePort port)
        {
            var portStyle = NodeEditorGUILayout.GetDefaultPortStyle(port);
            var uniNode = port.Node as UniNode;
            
            if (!uniNode) return portStyle;

            var portValue = uniNode.GetPort(port.FieldName);
            var hasData = portValue != null && portValue.HasValue;

            portStyle.Name       = port.FieldName;
            portStyle.Background = Color.red;
            portStyle.Color = port.Direction == PortIO.Input ? 
                hasData ? new Color(128, 128, 0) : Color.green :
                hasData ? new Color(128, 128, 0) : Color.blue;

            return portStyle;
        }
 
    }
}
