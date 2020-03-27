using GraphProcessor;
using UnityEngine.UIElements;

namespace UniGame.UniNodes.GameFlowEditor.Editor
{
    using UniGame.GameFlowEditor.Editor;
    using UnityEngine;

    public class UniGraphSettingsPinnedView : PinnedElementView
    {
        protected GameFlowGraphView graphView;
    
        readonly string exposedParameterViewStyle = "GraphProcessorStyles/ExposedParameterView";

        public UniGraphSettingsPinnedView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>(exposedParameterViewStyle));
        }
        
        protected override void Initialize(BaseGraphView graphView)
        {
            title = string.Empty;
            
            this.graphView = graphView as GameFlowGraphView;
            
            var runButton = new Button(ReloadGraphView) {
                name = "ReloadAction", 
                text = "Reload"
            };
            
            var saveButton = new Button(ReloadGraphView) {
                name = "SaveAction", 
                text = "Save"
            };
        
            content.Add(runButton);
            content.Add(saveButton);
        }

        private void ReloadGraphView()
        {
            
        }
        
        private void SaveGraphView()
        {
            graphView.Save();
        }
    }
}
