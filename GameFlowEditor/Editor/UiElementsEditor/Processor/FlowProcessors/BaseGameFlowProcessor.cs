namespace UniModules.UniGame.GameFlow.GameFlowEditor.Editor.UiElementsEditor.Processor.FlowProcessors
{
    using System;
    using System.Collections.Generic;
    using Abstract;
    using Core.Editor.EditorProcessors;
    using UiToolkit.Runtime.Extensions;
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
