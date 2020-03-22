namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.Drawers
{
    using System;
    using Runtime.Interfaces;
    using UnityEditor;

    public struct PropertyEditorData
    {
        public Type               Type;
        public string             Name;
        public string             Tooltip;
        public object             Target;
        public object             Source;
        public SerializedProperty Property;
    }
}