namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.Drawers.Interfaces
{
    using BaseEditor.Interfaces;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Interfaces;

    public interface INodeEditorHandler
    {
        
        bool Update(INodeEditorData editor,INode node);
        
    }
}