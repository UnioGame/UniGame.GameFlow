namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.Drawers.Interfaces
{
    using BaseEditor.Interfaces;
    using Runtime.Core;
    using Runtime.Interfaces;

    public interface INodeEditorHandler
    {
        
        bool Update(INodeEditorData editor,INode node);
        
    }
}