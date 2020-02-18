namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.Drawers.Interfaces
{
    using BaseEditor.Interfaces;
    using Runtime.Core;

    public interface INodeEditorHandler
    {
        
        bool Update(INodeEditorData editor,Node node);
        
    }
}