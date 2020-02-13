namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.Drawers.ReactivePortDrawers
{
    using System;
    using Runtime.Attributes;
    using Runtime.Core.Interfaces;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    [CustomPropertyDrawer(typeof(ReactivePortAttribute))]
    public class ReactivePortDrawer : PropertyDrawer
    {
        private const string ValueFieldName = "value";

        private GUIStyle errorStyle;
        
        public GUIStyle GetErrorStyle()
        {
            if (errorStyle != null)
                return errorStyle;
            
            errorStyle = new GUIStyle(GUI.skin.textField);
            errorStyle.normal.textColor = Color.red;
            return errorStyle;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
        
            // Create property container element.
            var container = property;
            var reactiveValue = container.FindPropertyRelative(ValueFieldName);

            // Create property fields.
            if (reactiveValue == null) {
                var target = property;
                EditorGUI.PropertyField(position, target,new GUIContent($"{target.displayName}: NULL Value"),true);
                return;    
            }
            
            EditorGUI.BeginProperty(position, label, property);
        
            if (EditorGUI.PropertyField(position, reactiveValue, label)) {
                property.serializedObject.ApplyModifiedProperties(); 
            }
        
            EditorGUI.EndProperty();
            
        }

        
        public static void DrawPropertyValue(object value,SerializedProperty sourceProperty, GUIContent label)
        {
            var property = sourceProperty;
            
            if (value is IReactiveSource) {
                property = property.FindPropertyRelative(ValueFieldName);
            }

            if (EditorGUILayout.PropertyField(property, label)) {
                sourceProperty?.serializedObject.ApplyModifiedProperties(); 
            }
        
        }
    }
}