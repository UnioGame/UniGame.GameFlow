using System.Collections;
using System.Collections.Generic;
using UniGame.UniNodes.NodeSystem.Inspector.Editor.ContentContextWindow;
using UniGreenModules.UniCore.Runtime.Common;
using UnityEngine;

[CreateAssetMenu(menuName = "UniGame/GameSystem/Examples/ContextDataWindowData",fileName = "ContextDataWindowData")]
public class DemoTypeDataContainer : ScriptableObject
{
    public TypeData data = new TypeData();

    public ScriptableObject so;

    public List<int> intListValue = new List<int>(){5,4,33,222,1};

    public GameObject go;

    public Sprite sprite;

    public Texture texture;

    public string stringValue;

    public Vector2 vector2Value;

    public Vector3 vector3Value;
    
    [ContextMenu("ShowContextWindow")]
    public void ShowContextWindow()
    {
        Show();
    }

    [Sirenix.OdinInspector.Button]
    public void Show()
    {
        data = new TypeData();
        data.Publish(so);
        data.Publish(intListValue);
        data.Publish(go);
        data.Publish(sprite);
        data.Publish(texture);
        data.Publish(vector2Value);
        data.Publish(vector3Value);
        data.Publish(stringValue);
        
        ContextContentWindow.Open(data);
    }
    
}
