namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor.Interfaces
{
    using System.Collections.Generic;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UniModules.UniCore.EditorTools.Editor.Interfaces;
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