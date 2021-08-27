using System;
using UniGame.GameFlowEditor.Runtime;
using UniModules.UniGame.Editor.DrawersTools;
using UnityEditor;

[CustomEditor(typeof(UniGraphAsset),true)]
public class UniGraphInspector : Editor
{
    private UniGraphAsset _asset;
    
    private void OnEnable()
    {
        _asset = target as UniGraphAsset;
    }

    public override void OnInspectorGUI()
    {

#if ODIN_INSPECTOR
        _asset?.DrawOdinPropertyInspector();
        return;
#endif
    
        base.OnInspectorGUI();
    }
}
