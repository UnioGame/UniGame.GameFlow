namespace UniGame.UniNodes.GameFlowEditor.Editor
{
    using System;
    using UnityEngine.UIElements;

    public interface IUniGraphSettings
    {
        void AddElement(VisualElement visualElement);
        void AddButton(string name, string title, Action action);
    }
}