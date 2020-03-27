using GraphProcessor;

namespace UniGame.UniNodes.GameFlowEditor.Editor
{
    using UnityEngine.UIElements;

    public class UniGraphToolbarView : ToolbarView
    {
        public UniGraphToolbarView(BaseGraphView graphView) : 
            base(graphView)
        {
            
        }
        
        protected override void AddButtons()
        {
            var conditionalProcessorVisible = graphView.GetPinnedElementStatus< UniGraphSettingsPinnedView >() != 
                                               DropdownMenuAction.Status.Hidden;
            AddToggle("Show Conditional Processor", conditionalProcessorVisible, 
                (v) => graphView.ToggleView< UniGraphSettingsPinnedView>());
        }
    }
}
