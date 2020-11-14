using GraphProcessor;

namespace UniGame.UniNodes.GameFlowEditor.Editor
{
    using UnityEngine.UIElements;

    public class UniGraphToolbarView : ToolbarView
    {
        private const string ActionMenu = "Show Actions";
        
        public UniGraphToolbarView(BaseGraphView graphView) : 
            base(graphView)
        {
            
        }
        
        protected override void AddButtons()
        {
            var isVisible = graphView.GetPinnedElementStatus< UniGraphSettingsPinnedView >() != 
                                               DropdownMenuAction.Status.Hidden;
            
            AddToggle(
                ActionMenu, 
                isVisible,
                (v) => graphView.ToggleView< UniGraphSettingsPinnedView>());
            
        }
    }
}
