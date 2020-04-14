namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor.Interfaces
{
    using System.Collections.Generic;
    using Runtime.Core;
    using Runtime.Interfaces;
    using UniGreenModules.UniCore.EditorTools.Editor.Interfaces;
    using UnityEditor;
    using UnityEngine;

    public interface INodeEditorData 
    {

        EditorNode EditorNode { get; }

        SerializedObject SerializedObject { get; }

        INode Target { get; }
        
        IReadOnlyDictionary<INodePort, Vector2> Ports { get; }

    }
}