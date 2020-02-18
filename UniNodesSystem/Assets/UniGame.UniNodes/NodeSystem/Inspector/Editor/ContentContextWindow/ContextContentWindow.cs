namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.ContentContextWindow
{
    using Runtime.Core;
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
            var portValue = node.GetPort(portName);
            if (portValue == null) return;
            
        }
    }
}
