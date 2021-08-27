namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector
{
    using System.Collections.Generic;
    using System.Reflection;
    using BaseEditor;
    using BaseEditor.Extensions;
    using BaseEditor.Interfaces;
    using GameFlowEditor.Editor;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Core.Interfaces;
    using UniModules.GameFlow.Runtime.Extensions;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UnityEditor;
    using UnityEngine;

    public static class UniNodeEditorExtensions
    {
        public static GUILayoutOption[] DefaultPortOptions = new GUILayoutOption[0];

        public static GUILayoutOption[] MainPortOptions = new GUILayoutOption[0];
        
        public static List<EditorNode> GetEditorNodes(this SerializedObject source,SerializedProperty property, IReadOnlyList<INode> nodes)
        {
            var items = new List<EditorNode>();
            for (var i = 0; i < nodes.Count; i++) {
                var node = nodes[i];
                if (node == null || !property.isArray || property.arraySize <= i) {
                    continue;
                }
                
                var editorNode = new EditorNode() {
                    Node     = node,
                    Source    = (node is Object nodeAsset) ? new SerializedObject(nodeAsset) : null,
                    Parent   = property,
                    Property = property.GetArrayElementAtIndex(i),
                };

                items.Add(editorNode);
            }
            return items;
        }

        
        public static NodePort DrawPortField(this NodePort port, GUIContent label, GUILayoutOption[] options)
        {
            NodeEditorGUILayout.PortField(label, port, options);
            return port;
        }

        public static INodePort DrawPortField(this INodePort port, NodeGuiLayoutStyle style)
        {
            NodeEditorGUILayout.PortField(port, style);

            return port;
        }

        public static void DrawPortPairField(
            this INode node,
            INodePort input,
            INodePort output,
            NodeGuiLayoutStyle intputStyle,
            NodeGuiLayoutStyle outputStyle)
        {
            NodeEditorGUILayout.PortPair(input, output, intputStyle, outputStyle);
        }

        public static NodePort DrawPortField(this NodePort port, GUILayoutOption[] options)
        {
            NodeEditorGUILayout.PortField(null, port, options);
            return port;
        }
    }
}