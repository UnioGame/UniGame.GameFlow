namespace UniGreenModules.UniNodeSystem.Inspector.Editor.BaseEditor.Interfaces
{
    using System.Collections.Generic;
    using Runtime.Core;
    using Runtime.Interfaces;
    using UniCore.EditorTools.Editor.Interfaces;
    using UnityEngine;

    public interface INodeEditorData : IEditorItem
    {
        
        int GetWidth();
        Color GetTint();
        GUIStyle GetBodyStyle();
        void Rename(string newName);

        bool IsSelected();
        
        INode Target { get; }

        IReadOnlyDictionary<NodePort, Vector2> HandledPorts { get; }

    }
}