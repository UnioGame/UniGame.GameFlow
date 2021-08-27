using GraphProcessor;
using UnityEngine.UIElements;

namespace UniGame.UniNodes.GameFlowEditor.Editor
{
    using System;
    using UniGame.GameFlowEditor.Editor;

#if ODIN_INSPECTOR
    using UniModules.UniGameFlow.GameFlowEditor.Editor.NodesSelectorWindow;
    using UnityEditor;
#endif

    using UnityEngine;

    public class UniGraphSettingsPinnedView : PinnedElementView, IUniGraphSettings
    {
        private const string ReloadAction = "ReloadAction";
        private const string NodesAction  = "ShowNodes";
        private const string SaveAction   = "SaveAction";

        private const string ReloadText = "Reload";
        private const string SaveText   = "Save";
        private const string NodesText  = "Show Nodes";

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
            var runButton = new Button(action)
            {
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

            AddButton(ReloadAction, ReloadText, ReloadGraphView);
            AddButton(SaveAction, SaveText, ReloadGraphView);
#if ODIN_INSPECTOR
            AddButton(NodesAction, NodesText, ShowNodesWindow);
#endif
            
        }

#if ODIN_INSPECTOR
        private void ShowNodesWindow()
        {
            NodesInfoWindow.ShowWindow();
        }
#endif
        
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