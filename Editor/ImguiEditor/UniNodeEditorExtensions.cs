namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector
{
    using System.Collections.Generic;
    using BaseEditor;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UnityEditor;
    using UnityEngine;

    public static class UniNodeEditorExtensions
    {
        public static GUILayoutOption[] DefaultPortOptions = new GUILayoutOption[0];

        public static GUILayoutOption[] MainPortOptions = new GUILayoutOption[0];
        
        public static List<EditorNodeData> GetEditorNodes(this SerializedObject source,SerializedProperty property, IReadOnlyList<INode> nodes)
        {
            var items = new List<EditorNodeData>();
            for (var i = 0; i < nodes.Count; i++) {
                var node = nodes[i];
                if (node == null || !property.isArray || property.arraySize <= i) {
                    continue;
                }
                
                var editorNode = new EditorNodeData() {
                    Node     = node,
                    Source    = (node is Object nodeAsset) ? new SerializedObject(nodeAsset) : null,
                    Parent   = property,
                    Property = property.GetArrayElementAtIndex(i),
                };

                items.Add(editorNode);
            }
            return items;
        }
    }
}