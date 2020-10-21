namespace UniModules.UniGame.GameFlow.GameFlowEditor.Editor.NodesSelectorWindow.OdinWindow {
    using System;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    public class NodeInfoData : Sirenix.OdinInspector.ISearchFilterable {

        private const int labelWidth = 100;
        
        [Space]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.LabelWidth(labelWidth)]
#endif
        public string Name = string.Empty;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.LabelWidth(labelWidth)]
#endif
        public string Category = string.Empty;
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.LabelWidth(labelWidth)]
#endif
        public MonoScript Script;

        [Space]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.LabelWidth(labelWidth)]
        [Sirenix.OdinInspector.MultiLineProperty(5)]       
        [Sirenix.OdinInspector.ReadOnly]
        [Sirenix.OdinInspector.HideIf("@this.Description == string.Empty")]
#endif
        public string Description = string.Empty;

        public bool IsMatch(string searchString) {

            var scriptName = this.Script ? this.Script.name : string.Empty;
            var scriptType = this.Script ? this.Script.GetClass().Name : string.Empty;
            
            var result = this.Name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
            result |= this.Category.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
            result |= this.Description.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
            result |= scriptName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
            result |= scriptType.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
            return result;
        }
    }
}