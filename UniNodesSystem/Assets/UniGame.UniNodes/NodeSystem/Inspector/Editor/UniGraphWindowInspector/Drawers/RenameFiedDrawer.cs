namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.Drawers
{
    using BaseEditor.Interfaces;
    using Interfaces;
    using Runtime.Core;
    using Runtime.Interfaces;
    using UnityEditor;

    public class RenameFiedDrawer : INodeEditorHandler
    {
    
        public bool Update(INodeEditorData editor, INode node)
        {
            var nodeName  = node.ItemName;
            var nameValue = EditorGUILayout.TextField("name:", nodeName);
            if (!string.Equals(nameValue, nodeName))
            {
                node.SetName(nameValue);
            }
        
            return true;
        }
    }
}
