namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.Drawers
{
    using BaseEditor;
    using BaseEditor.Interfaces;
    using Interfaces;
    using Runtime.Core;
    using UnityEditor;
    using UnityEngine;

    public class BaseHeaderDrawer : INodeEditorHandler
    {
        public virtual bool Update(INodeEditorData editor, Node node)
        {
            var target = node;
            var title = target.GetName();
            if (string.IsNullOrEmpty(title)) {
                CreateNodeMenuAttribute attrib;
                var type = node.GetType();
                title = NodeEditorUtilities.GetAttrib(type, out attrib) ? attrib.nodeName : type.Name;
                node.nodeName = title;
            }
            var renaming = NodeEditor.Renaming;

            if (NodeEditor.Renaming != 0 && Selection.Contains(target))
            {
                int controlId = GUIUtility.GetControlID(FocusType.Keyboard) + 1;
                if (renaming == 1)
                {
                    GUIUtility.keyboardControl = controlId;
                    EditorGUIUtility.editingTextField = true;
                    NodeEditor.Renaming = 2;
                }

                target.nodeName = EditorGUILayout.TextField(target.ItemName, NodeEditorResources.styles.nodeHeader,
                    GUILayout.Height(30));
                
                if (!EditorGUIUtility.editingTextField)
                {
                    editor.Rename(target.ItemName);
                    NodeEditor.Renaming = 0;
                }
            }
            else
            {
                GUILayout.Label(title, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
            }

            return true;
        }
    }
}