using System;
using UniGame.UniNodes.Nodes.Runtime.SerializableNodes;
using UniGame.UniNodes.NodeSystem.Runtime.Attributes;
using UniGame.UniNodes.NodeSystem.Runtime.Core;
using UniGame.UniNodes.NodeSystem.Runtime.Core.Nodes;
using UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
using UniGame.UniNodes.NodeSystem.Runtime.ReactivePorts;
using UnityEngine;

[Serializable]
[CreateNodeMenu("Examples/DemoFlowNode")]
public class DemoFlowNode : SNode
{
    [Port]
    public int intIn;
//    
//    [Port]
//    public int intIn1;
//    
//    [Port]
//    public int intIn2;
//      
//    [Port]
//    public int intIn3;
//    
    [Port(PortIO.Output)]
    public int intOut;
          
    [ReactivePort]
    public BoolReactivePort boolRIn = new BoolReactivePort();
    
    [ReactivePort(PortIO.Output)]
    public BoolReactivePort boolROut = new BoolReactivePort();

    public int intValue;
    public GameObject gameObject;    
    public Vector2Int Vector2Int;
    public Vector2 Vector2;
//    public Vector3 Vector3;
//    public Vector3Int Vector3Int;
//    public float floatValue;
//
//    public List<Object> AssetList = new List<Object>();
//    
//#if ODIN_INSPECTOR
//    [Sirenix.OdinInspector.InlineEditor]
//#endif
//    
//    public Component Component;
//    public Texture Texture;
//    public Sprite Sprite;
//    
//    public Object asset;
//    public Animator Animator;
//
//    [DrawWithUnity]
//    [ShowAssetReference]
//    public AssetReference AssetReference1;
//    
//    [DrawWithUnity]
//    [ShowAssetReference]
//    public AssetReferenceGameObject GameObjectAssetRef;
//    public AssetReferenceScriptableObject ScriptableObjectAssetReference;
//    public AssetReferenceSprite SpriteRefence1;
//    
    [SerializeReference]
    public INode LogNode = new SLogNode();

}
