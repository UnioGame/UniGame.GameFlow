using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UniGame.UniNodes.Nodes.Runtime.SerializableNodes;
using UniGame.UniNodes.NodeSystem.Runtime.Core;
using UniGame.UniNodes.NodeSystem.Runtime.Core.Nodes;
using UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
using UniGreenModules.UniGame.AddressableTools.Runtime.Attributes;
using UniGreenModules.UniGame.SerializableContext.Runtime.Addressables;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

[Serializable]
[CreateNodeMenu("Examples/DemoFlowNode")]
public class DemoFlowNode : SNode
{

    public int intValue;
    public GameObject gameObject;    
    public Vector2Int Vector2Int;
    public Vector2 Vector2;
    public Vector3 Vector3;
    public Vector3Int Vector3Int;
    public float floatValue;

    public List<Object> AssetList = new List<Object>();
    
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.InlineEditor]
#endif
    public Component Component;
    public Texture Texture;
    public Sprite Sprite;
    
    public Object asset;
    public Animator Animator;

    [DrawWithUnity]
    [ShowAssetReference]
    public AssetReference AssetReference1;
    
    [DrawWithUnity]
    [ShowAssetReference]
    public AssetReferenceGameObject GameObjectAssetRef;
    public AssetReferenceScriptableObject ScriptableObjectAssetReference;
    public AssetReferenceSprite SpriteRefence1;
    
    [SerializeReference]
    public INode LogNode = new SLogNode();

}
