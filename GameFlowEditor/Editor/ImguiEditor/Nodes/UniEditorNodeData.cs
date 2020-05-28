namespace UniGame.GameFlowEditor.Editor
{
    using System.Collections.Generic;
    using UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor;
    using UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor.Interfaces;
    using UniNodes.NodeSystem.Runtime.Interfaces;
    using UnityEditor;
    using UnityEngine;

    public class UniEditorNodeData : INodeEditorData
    {
        public EditorNode                     editorNode;
        public SerializedObject               nodeObject;
        public INode                          node;
        public Dictionary<INodePort, Vector2> ports;

        public EditorNode                              EditorNode       => editorNode;
        public SerializedObject                        SerializedObject => nodeObject;
        public INode                                   Target           => node;
        public IReadOnlyDictionary<INodePort, Vector2> Ports            => ports;
    }
}