namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.ContentContextWindow
{
    using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Nodes;
    using UnityEditor;

    public class ContextContentWindow : EditorWindow
    {
        private UniNode node;
        private string portName;
        public static void Open(UniNode node, string portName)
        {
            var window = GetWindow<ContextContentWindow>(); 
            window.Initialize(node,portName);
            window.Show();
        }

        public void Initialize(UniNode node, string portName)
        {
            var portValue = node.GetPortValue(portName);
            if (portValue == null) return;
            
        }
    }
}
