namespace UniGame.UniNodes.GameFlowEditor.Editor.Drawers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.EditorTools.Editor.UiElements;
    using Core.Runtime.Attributes.FieldTypeDrawer;
    using NodeSystem.Inspector.Editor.UniGraphWindowInspector;
    using NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor;
    using NodeSystem.Inspector.Editor.UniGraphWindowInspector.Drawers;
    using NodeSystem.Runtime.Core;
    using NodeSystem.Runtime.Interfaces;
    using UniGreenModules.UniGame.Core.EditorTools.Editor.DrawersTools;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    [UiElementsDrawer(1000)]
    public class ImGuiNodeFieldsDrawer : IUiElementsTypeDrawer
    {
        private static Dictionary<INode,EditorNode> nodeDataCache = 
            new Dictionary<INode, EditorNode>(16);
        
        
        private NodeFieldsContainer fiedlsContainer = new NodeFieldsContainer();

        public bool IsTypeSupported(Type type)
        {
            return typeof(INode).IsAssignableFrom(type);
        }

        public VisualElement Draw(
            object source,
            Type type,
            string label = "",
            Action<object> onValueChanged = null)
        {
            var backgroundColor = new Color(0.5f, 0.5f, 0.5f);
            var node = source as INode;
            var container = new Foldout() {
                text = string.IsNullOrEmpty(label) ? "content" : label,
                value = false,
                style = {
                    paddingLeft     = 4,
                    color           = new StyleColor(Color.black),
                    backgroundColor = new StyleColor(backgroundColor)
                }
            };
            
            var view = DrawNode(node,onValueChanged);
            view.style.backgroundColor = new StyleColor(backgroundColor);
            
            container?.Add(view);
            
            return container;
        }
        
        public EditorNode GetData(INode node)
        {
            if (nodeDataCache.TryGetValue(node, out var data))
                return data;
            
            UpdateCache(node.GraphData as UniGraph);
            
            return nodeDataCache[node];
        }

        public VisualElement DrawNode(INode node,Action<object> onValueChanged)
        {
            VisualElement element = null;
            
#if ODIN_INSPECTOR
            element = new IMGUIContainer(() => node.DrawOdinPropertyInspector());
#else
            element = new VisualElement();
#endif
            var data = GetData(node);
            var fields = fiedlsContainer.GetFields(data);
            
            foreach (var field in fields) {
                var view = DrawField(field);
                view.style.marginBottom = 4;
                element.Add(view);
            }
            
            //save source object
            data.Property.serializedObject.
                ApplyModifiedPropertiesWithoutUndo();

            return element;
        }

        private VisualElement DrawField(PropertyEditorData field)
        {
            var property = field.Property;
            var label = new GUIContent(field.Name, field.Tooltip);
            
            if (property.propertyType == SerializedPropertyType.ObjectReference && 
                (property.objectReferenceValue is GameObject) == false) {
                var odinFieldView = new OdinFieldView() {
                    Label = label,
                    Property = property,
                    IsOpen = false,
                };
                return odinFieldView.View;
            }
            return DrawImGuiField(field,label);
        }

        private IMGUIContainer DrawImGuiField(PropertyEditorData field,GUIContent label)
        {
            var property = field.Property;
            

            var imGuiContainer = new IMGUIContainer(() => {
                var color = GUI.contentColor;
                GUI.color = Color.white;
                EditorGUIUtility.labelWidth = 84;
                EditorGUILayout.PropertyField(property, label, true);
                GUI.color = color;
            });
            return imGuiContainer;
        }
        
        private void UpdateCache(UniGraph sourceGraph)
        {
            var sourceObject = new SerializedObject(sourceGraph);
            
            var serializableNodesProperty = sourceObject.
                FindProperty(nameof(sourceGraph.serializableNodes));
            var assetNodesProperty = sourceObject.
                FindProperty(nameof(sourceGraph.nodes));
            
            var serializableNodes = sourceObject.
                GetEditorNodes(serializableNodesProperty,sourceGraph.serializableNodes);
            var assetNodes = sourceObject.
                GetEditorNodes(assetNodesProperty,sourceGraph.nodes);

            foreach (var data in serializableNodes.Concat(assetNodes)) {
                nodeDataCache[data.Node] = data;
            }
        }
        
    }
}
