namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.Drawers
{
    using System;
    using BaseEditor.Interfaces;
    using Interfaces;
    using Runtime.Core;
    using UniGreenModules.UniCore.EditorTools.Editor.Utility;

    public class ButtonActionBodyDrawer : INodeEditorHandler
    {
        private readonly string _label;
        private readonly Action _action;

        public ButtonActionBodyDrawer(string label,Action action)
        {
            _label = label;
            _action = action;
        }
    
        public bool Update(INodeEditorData editor, Node node)
        {
           
            EditorDrawerUtils.DrawButton(_label,_action);

            return true;
        }
    
    }
}
