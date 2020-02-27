using System;
using System.Collections;
using System.Collections.Generic;
using UniGame.UniNodes.NodeSystem.Inspector.Editor.ContentContextWindow;
using UniGreenModules.UniCore.Runtime.Common;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class DemoContextClass
{
    public int One = 3;

    public float FloatValue = 4f;

    public ScriptableObject SOValue;

    public string Text = "DemoContextClass Text0";
}

[CreateAssetMenu(menuName = "UniGame/GameSystem/Examples/ContextDataWindowData",fileName = "ContextDataWindowData")]
public class DemoTypeDataContainer : ScriptableObject
{
    public TypeData data = new TypeData();

    public ScriptableObject so;

    public List<int> intListValue = new List<int>(){5,4,33,222,1};

    public List<Vector3> vectorItems = new List<Vector3>();
    
    public List<DemoTypeDataContainer> soItems = new List<DemoTypeDataContainer>();
    
    public List<GameObject> gameObjects = new List<GameObject>();
    
    public List<Object> objects = new List<Object>();
    
    public GameObject go;

    public Sprite sprite;

    public Texture texture;

    public string stringValue;

    public Vector2 vector2Value;

    public Vector3 vector3Value;

    public DemoContextClass serializableClassValue = new DemoContextClass();
    
    [ContextMenu("ShowContextWindow")]
    public void ShowContextWindow()
    {
        Show();
    }

    [Sirenix.OdinInspector.Button]
    public void Show()
    {
        data = new TypeData();
        data.Publish(serializableClassValue);
        data.Publish(so);
        data.Publish(intListValue);
        data.Publish(go);
        data.Publish(sprite);
        data.Publish(texture);
        data.Publish(vector2Value);
        data.Publish(vector3Value);
        data.Publish(stringValue);
        data.Publish(gameObjects);
        data.Publish(objects);
        data.Publish(soItems);
        data.Publish(vectorItems);
        
        ContextContentWindow.Open(data);
    }
    
}
