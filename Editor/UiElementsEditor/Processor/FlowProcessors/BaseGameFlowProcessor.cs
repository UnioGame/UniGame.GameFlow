namespace UniModules.GameFlow.Editor.Processor.FlowProcessors
{
    using UniModules.UniGame.Core.Editor.EditorProcessors;
    using UniModules.UniGame.UiToolkit.Runtime.Extensions;

    
    using System;
    using System.Collections.Generic;
    using Abstract;
    using UnityEngine.UIElements;

    [Serializable]
    public class BaseGameFlowProcessor : BaseEditorProcessorAsset<UniGameFlowWindow>,IGameFlowGraphProcessor
    {
        public StyleSheet styleSheet;

        public override void Proceed(IReadOnlyList<UniGameFlowWindow> data)
        {
            foreach (var flowWindow in data)
            {
                OnFlowWindow(flowWindow);
            }
        }
        
        private void OnFlowWindow(UniGameFlowWindow window)
        {
            var root = window.rootVisualElement;

            root.AddStyleSheet(styleSheet);
            
            if (window.IsActiveGraph){
                root.SwapClasses(GameFlowStyleConstants.disabledWindowGroup, GameFlowStyleConstants.activeWindowGroup);
            }
            else {
                root.SwapClasses(GameFlowStyleConstants.activeWindowGroup, GameFlowStyleConstants.disabledWindowGroup);
            }
        }

    }
}
