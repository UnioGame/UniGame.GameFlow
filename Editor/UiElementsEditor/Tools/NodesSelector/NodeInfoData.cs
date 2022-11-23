#if ODIN_INSPECTOR

namespace UniModules.UniGame.GameFlow.GameFlowEditor.Editor.NodesSelectorWindow.OdinWindow
{
    using System;
    using global::UniGame.Core.Runtime.SerializableType;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    public class NodeInfoData
#if ODIN_INSPECTOR_3
            : Sirenix.OdinInspector.ISearchFilterable
#endif

    {
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

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.LabelWidth(labelWidth)]
#endif
        public string MenuName;

        public SType NodeType;
        
        [Space]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.LabelWidth(labelWidth)]
        [Sirenix.OdinInspector.MultiLineProperty(5)]
        [Sirenix.OdinInspector.ReadOnly]
        [Sirenix.OdinInspector.HideIf("@this.Description == string.Empty")]
#endif
        public string Description = string.Empty;

        public bool IsMatch(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
                return true;
            var scriptName = Script ? Script.name : string.Empty;
            var scriptType = Script ? Script.GetClass().Name : string.Empty;

            var result = Name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
            result |= Category.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
            result |= Description.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
            result |= scriptName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
            result |= scriptType.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
            result |= MenuName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
            return result;
        }
    }
}

#endif