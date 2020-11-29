namespace UniModules.UniGame.GameFlow.GameFlowEditor.Editor.UiElementsEditor.Processor.FlowProcessors
{
    using System;
    using System.Collections.Generic;
    using Abstract;
    using Core.Editor.EditorProcessors;
    using Core.Runtime.DataStructure;
    using Core.Runtime.SerializableType;
    using Core.Runtime.SerializableType.Attributes;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
    using UnityEngine;
    using UnityEngine.UIElements;


    public class NodeTypeStyleEditorProcessor : BaseEditorProcessorAsset<NodeTypeStyleEditorProcessor>,IGameFlowGraphProcessor
    {
        public StyleMap nodeStyleMap = new StyleMap();
        
        [STypeFilter(typeof(INode),true)]
        public SType nodeType;
        
        public StyleSheet style;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void AddStyle()
        {
            if (nodeType == null || style == null)
            {
                Debug.LogError($"Argument NULL");
                return;
            }

            nodeStyleMap[nodeType] = style;
        }
        
        public void Proceed(IReadOnlyList<UniGameFlowWindow> data)
        {
            
        }
    }

    [Serializable]
    public class StyleMap : SerializableDictionary<SType, StyleSheet>
    {
        
    }
    
}
