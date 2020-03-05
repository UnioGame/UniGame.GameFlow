namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.Drawers
{
    using System;
    using System.Collections.Generic;
    using BaseEditor;
    using BaseEditor.Interfaces;
    using Interfaces;
    using Runtime.Attributes;
    using Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.ReflectionUtils;
    using UnityEditor;
    using UnityEngine;

    public class BaseBodyDrawer : INodeEditorHandler
    {
        private List<string> _excludes;

        public BaseBodyDrawer()
        {
            //TODO remove this old dirty hack
            _excludes = new List<string>() {"m_Script", "position", "ports", "id"};
        }

        public bool Update(INodeEditorData editor, INode node)
        {
            EditorGUIUtility.labelWidth = 84;

            foreach (var item in GetNodeItems(editor, node)) {
                if (!IsItemVisible(item.Type, item.Name))
                    continue;
                DrawItem(item);
            }

            return true;
        }

        public virtual void DrawItem(NodeItemEditorData item)
        {
            var node = item.Node;
            node.DrawNodePropertyField(
                item.Property,
                new GUIContent(item.Name, item.Tooltip),
                true);
        }

        private int counter = 0;

        public virtual IEnumerable<NodeItemEditorData> GetNodeItems(INodeEditorData editor, INode node)
        {
            var editorNode       = editor.EditorNode;
            var serializedObject = editor.SerializedObject;

            var targetProperty = serializedObject == null ? 
                editorNode.Property : 
                serializedObject.GetIterator();

            var property = targetProperty.Copy();
            
            var type = node.GetType();

            EditorGUIUtility.labelWidth = 84;

            var moveNext = property.NextVisible(true);

            while (moveNext && !property.Equals(targetProperty))
            {
                yield return new NodeItemEditorData() {
                    Node     = node,
                    Name     = property.name,
                    Tooltip  = property.tooltip,
                    Source   = node,
                    Type     = type,
                    Property = property,
                };
                moveNext = property.NextVisible(false);
            }

            counter++;
        }

        public bool IsItemVisible(Type type, string fieldName)
        {
            //is node field should be draw
            var field         = type.GetFieldInfoCached(fieldName);
            var hideInspector = field?.GetCustomAttributes(typeof(HideNodeInspectorAttribute), false).Length > 0;
            return !hideInspector && !_excludes.Contains(fieldName);
        }
    }
}