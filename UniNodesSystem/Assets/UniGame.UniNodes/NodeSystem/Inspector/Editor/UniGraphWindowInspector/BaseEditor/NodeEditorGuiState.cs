namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor
{
    using System.Collections.Generic;
    using UnityEngine;

    public struct NodeEditorGuiState
    {
        public Event        Event;
        public Vector2      MousePosition;
        public List<int>    PreSelection;
        public EventType    EventType;
    }
}