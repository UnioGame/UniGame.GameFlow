namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.Styles
{
    using BaseEditor;
    using Runtime.Core;
    using UnityEngine;

    public class PortStyleSelector : IPortStyleProvider
    {
    
        public virtual NodeGuiLayoutStyle Select(NodePort port)
        {
            var portStyle = NodeEditorGUILayout.GetDefaultPortStyle(port);
            var uniNode = port.Node as UniNode;
            
            if (!uniNode) return portStyle;

            var portValue = port.Value;
            var hasData = portValue != null && portValue.HasValue;

            portStyle.Name       = port.ItemName;
            portStyle.Background = port.Direction == PortIO.Input ? 
                hasData ? new Color(128, 128, 0) : Color.green :
                hasData ? new Color(128, 128, 0) : Color.blue;
            portStyle.Color = NodeEditorPreferences.GetTypeColor(port.ValueType);
            
            return portStyle;
        }
 
    }
}
