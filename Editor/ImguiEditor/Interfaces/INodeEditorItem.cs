namespace UniModules.UniCore.EditorTools.Editor.Interfaces
{
    using System.Collections.Generic;
    using global::UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor.Interfaces;
    using global::UniModules.GameFlow.Runtime.Interfaces;
    using UnityEngine;

    public interface INodeEditorItem : INodeEditorData
    {
        
        void Rename(string newName);

        bool IsSelected { get; set; }

        int GetWidth();
        
        Color GetTint();
        
        GUIStyle GetBodyStyle();

        IReadOnlyDictionary<INodePort, Vector2> HandledPorts { get; }

    }
}
