namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor.Interfaces
{
    using System.Collections.Generic;
    using Runtime.Core;
    using Runtime.Interfaces;
    using UniGreenModules.UniCore.EditorTools.Editor.Interfaces;
    using UnityEditor;
    using UnityEngine;

    public interface INodeEditorData : IEditorItem
    {
        
        int GetWidth();
        
        Color GetTint();
        
        GUIStyle GetBodyStyle();
        
        void Rename(string newName);

        bool IsSelected { get; set; }

        EditorNode EditorNode { get; }

        SerializedObject SerializedObject { get; }

        INode Target { get; }

        IReadOnlyDictionary<NodePort, Vector2> HandledPorts { get; }

    }
}