using GraphProcessor;
using UnityEngine.UIElements;

namespace UniGame.UniNodes.GameFlowEditor.Editor
{
    using System;
    using UniGame.GameFlowEditor.Editor;
    using UnityEditor;
    using UnityEngine;

    public class UniGraphSettingsPinnedView : PinnedElementView, IUniGraphSettings
    {
        protected GameFlowGraphView graphView;
    
        readonly string exposedParameterViewStyle = "GraphProcessorStyles/ExposedParameterView";

        public UniGraphSettingsPinnedView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>(exposedParameterViewStyle));
        }
        
        #region public methods

        public void AddElement(VisualElement visualElement) => content.Add(visualElement);

        public void AddButton(string name, string title, Action action)
        {
            var runButton = new Button(action) {
                name = name, 
                text = title
            };
            AddElement(runButton);
        }
        
        #endregion
        
        
        protected override void Initialize(BaseGraphView graphView)
        {
            title = string.Empty;
            
            this.graphView = graphView as GameFlowGraphView;

            AddButton("ReloadAction","Reload",ReloadGraphView);
            //TODO fix 
            AddButton("SaveAction","Save",ReloadGraphView);
        }

        private void ReloadGraphView()
        {
            SaveGraphView();
            graphView.GameFlowWindow.Reload();
        }
        
        private void SaveGraphView()
        {
            graphView.GameFlowWindow.Save();
        }
    }
}
