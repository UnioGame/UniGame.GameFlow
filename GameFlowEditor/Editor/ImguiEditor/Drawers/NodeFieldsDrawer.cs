namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.Drawers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BaseEditor;
    using Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.ReflectionUtils;
    using UniGreenModules.UniGame.Core.Runtime.Attributes.FieldTypeDrawer;
    using UnityEditor;
    using UnityEngine;

    public class NodeFieldsContainer
    {
        
        private static Dictionary<INode,List<PropertyEditorData>> _cachedProperties = 
            new Dictionary<INode, List<PropertyEditorData>>(16);

        
        private List<string> _excludes = new List<string>() {"m_Script", "position", "ports", "id"};

        public NodeFieldsContainer()
        {
            AssemblyReloadEvents.beforeAssemblyReload += _cachedProperties.Clear;
        }
        
        public IReadOnlyList<PropertyEditorData> GetFields(EditorNode editorNode)
        {
            var serializedObject = editorNode.Source;
            var parent           = editorNode.Parent;
            var node             = editorNode.Node;

            if (_cachedProperties.TryGetValue(node, out var propertyEditorData))
                return propertyEditorData;
            
            var targetProperty = serializedObject == null ? 
                editorNode.Property : 
                serializedObject.GetIterator();

            var items = node.
                GetProperties(targetProperty, parent).
                Where(x => IsItemVisible(x.Type, x.Name)).
                ToList();

            _cachedProperties[node] = items;
            
            return items;
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