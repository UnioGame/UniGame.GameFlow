namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.Styles
{
    using BaseEditor;
    using Runtime.Core;
    using Runtime.Interfaces;
    using UnityEngine;

    public class PortStyleSelector : IPortStyleProvider
    {
    
        public virtual NodeGuiLayoutStyle Select(INodePort port)
        {
            var portStyle = NodeEditorGUILayout.GetDefaultPortStyle(port);

            if (!(port.Node is INode uniNode)) return portStyle;

            var portValue = port.Value;
            var hasData = portValue != null && portValue.HasValue;

            portStyle.Name       = port.ItemName;
            portStyle.Background = port.Direction == PortIO.Input ? 
                hasData ? new Color(128, 128, 0) : Color.green :
                hasData ? new Color(128, 128, 0) : Color.blue;
            portStyle.Color = GameFlowPreferences.GetTypeColor(port.ValueType);
            
            return portStyle;
        }
 
    }
}
