namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.Drawers
{
    using BaseEditor;
    using BaseEditor.Interfaces;
    using Interfaces;
    using Runtime.Core;
    using Runtime.Interfaces;
    using UniModules.Editor;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UnityEditor;
    using UnityEngine;

    public class BaseHeaderDrawer : INodeEditorHandler
    {
        public virtual bool Update(INodeEditorData editor, INode node)
        {
            var target = node;
            var title = target.ItemName;
            if (string.IsNullOrEmpty(title)) {
                CreateNodeMenuAttribute attrib;
                var type = node.GetType();
                title = NodeEditorUtilities.GetAttrib(type, out attrib) ? attrib.nodeName : type.Name;
                node.SetName(title);
            }
            var renaming = NodeEditor.Renaming;

            if (NodeEditor.Renaming != 0 && target.IsSelected())
            {
                var controlId = GUIUtility.GetControlID(FocusType.Keyboard) + 1;
                if (renaming == 1)
                {
                    GUIUtility.keyboardControl = controlId;
                    EditorGUIUtility.editingTextField = true;
                    NodeEditor.Renaming = 2;
                }

                var nodeName = EditorGUILayout.TextField(target.ItemName, NodeEditorResources.styles.nodeHeader,
                    GUILayout.Height(30));
                target.SetName(nodeName);

                if (EditorGUIUtility.editingTextField) {
                    return true;
                }

                node.SetName(target.ItemName);
                NodeEditor.Renaming = 0;
            }
            else
            {
                GUILayout.Label(title, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
            }

            return true;
        }
    }
}