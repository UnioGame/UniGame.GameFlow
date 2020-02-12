namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.Drawers.ReactivePortDrawers
{
    // using Runtime.ReactivePorts;
    //
    // /// <summary>
    // /// base drawer class for ReactivePortValue<T>
    // /// </summary>
    // /// <typeparam name="T">Type of target Value</typeparam>
    // public class ReactivePortDrawer<T> : ReactivePortDrawer { }
    //
    // [UnityEditor.CustomPropertyDrawer(typeof(IntReactivePort))]
    // public class IntReactivePortDrawer : ReactivePortDrawer<int> { }
    //
    // [UnityEditor.CustomPropertyDrawer(typeof(FloatReactivePort))]
    // public class FloatReactivePortDrawer : ReactivePortDrawer<float> { }
    //
    // [UnityEditor.CustomPropertyDrawer(typeof(StringReactivePort))]
    // public class StringReactivePortDrawer : ReactivePortDrawer<string> { }
    //
    // [UnityEditor.CustomPropertyDrawer(typeof(ByteReactivePort))]
    // public class ByteReactivePortDrawer : ReactivePortDrawer<byte> { }
    //
    // [UnityEditor.CustomPropertyDrawer(typeof(BoolReactivePort))]
    // public class BoolReactivePortDrawer : ReactivePortDrawer<bool> { }
    //
    // [UnityEditor.CustomPropertyDrawer(typeof(ContextReactivePort))]
    // public class ContextReactivePortDrawer : ReactivePortDrawer<bool> { }
}

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.Drawers.ReactivePortDrawers
{
    using System;
    using DG.DemiEditor;
    using Runtime.Attributes;
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
            container = container.FindPropertyRelative(ValueFieldName);
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
            
            // if (value is IReactiveSource) {
            //     GameLog.Log($"DrawPropertyValue field {property?.name}");
            //     property = property.FindPropertyRelative(ValueFieldName);
            //     GameLog.Log($"DrawPropertyValue field {property?.name}");
            //     property = property.FindPropertyRelative(ValueFieldName);
            //     GameLog.Log($"DrawPropertyValue field {property?.name}");
            // }

            if (EditorGUILayout.PropertyField(property, label)) {
                sourceProperty?.serializedObject.ApplyModifiedProperties(); 
            }
        
        }
    }
}