namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.Drawers
{
    using System;
    using Runtime.Interfaces;
    using UnityEditor;

    public struct NodeItemEditorData
    {
        public Type               Type;
        public string             Name;
        public string             Tooltip;
        public INode              Node;
        public object             Source;
        public SerializedProperty Property;
    }
}