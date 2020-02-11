using System.Collections;
using System.Collections.Generic;
using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.ReactivePorts;
using UnityEditor;
using UnityEngine;

/// <summary>
/// base drawer class for ReactivePortValue<T>
/// </summary>
/// <typeparam name="T">Type of target Value</typeparam>
public class ReactivePortDrawer<T> : PropertyDrawer
{
    private const string valueFieldName = "value";
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        var reactiveValue = property.FindPropertyRelative(valueFieldName);
        if (reactiveValue == null)
            return;
        
        var containerValue = reactiveValue.FindPropertyRelative(valueFieldName);
        if (containerValue == null)
            return;
        
        if (EditorGUI.PropertyField(position, containerValue, label)) {
            property.serializedObject.ApplyModifiedProperties(); 
        }
        
        EditorGUI.EndProperty();
    }
    
}

//[UnityEditor.CustomPropertyDrawer(typeof(IntReactivePort))]
public class IntReactivePortDrawer : ReactivePortDrawer<int> { }

[UnityEditor.CustomPropertyDrawer(typeof(FloatReactivePort))]
public class FloatReactivePortDrawer : ReactivePortDrawer<float> { }

[UnityEditor.CustomPropertyDrawer(typeof(StringReactivePort))]
public class StringReactivePortDrawer : ReactivePortDrawer<string> { }

[UnityEditor.CustomPropertyDrawer(typeof(ByteReactivePort))]
public class ByteReactivePortDrawer : ReactivePortDrawer<byte> { }

[UnityEditor.CustomPropertyDrawer(typeof(BoolReactivePort))]
public class BoolReactivePortDrawer : ReactivePortDrawer<bool> { }
