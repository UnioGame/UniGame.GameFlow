namespace UniModules.UniGame.GameFlow.GameFlowEditor.Editor.UiElementsEditor
{
    using System;

    [Serializable]
    public class GameFlowStyleConstants
    {
        public const string CssResourcePath = "GameFlow/UCSS";
        
        public const string activeWindowGroup   = nameof(activeWindowGroup);
        public const string selectedWindowGroup = nameof(selectedWindowGroup);
        public const string disabledWindowGroup = nameof(disabledWindowGroup);

        public const string nodeTitle  = nameof(nodeTitle);
        public const string nodeActive = nameof(nodeActive);
        public const string inputPortActive = nameof(inputPortActive);
        public const string outputPortActive = nameof(outputPortActive);
        
    }
}