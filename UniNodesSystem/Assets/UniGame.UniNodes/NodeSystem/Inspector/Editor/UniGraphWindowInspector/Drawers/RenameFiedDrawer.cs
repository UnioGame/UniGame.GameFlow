namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.Drawers
{
    using BaseEditor.Interfaces;
    using Interfaces;
    using Runtime.Core;
    using UnityEditor;

    public class RenameFiedDrawer : INodeEditorHandler
    {
    
        public bool Update(INodeEditorData editor, Node node)
        {
            var nodeName  = node.GetName();
            var nameValue = EditorGUILayout.TextField("name:", nodeName);
            if (!string.Equals(nameValue, nodeName))
            {
                node.name = nameValue;
            }
        
            return true;
        }
    }
}
