namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor.Interfaces
{
    using System;

    public interface INodeEditorAttribute {
        Type GetInspectedType();
    }
}