namespace UniGreenModules.UniNodeSystem.Inspector.Editor.Drawers.Interfaces
{
    using BaseEditor.Interfaces;
    using Runtime.Core;

    public interface INodeEditorHandler
    {
        
        bool Update(INodeEditorData editor,UniBaseNode node);
        
    }
}