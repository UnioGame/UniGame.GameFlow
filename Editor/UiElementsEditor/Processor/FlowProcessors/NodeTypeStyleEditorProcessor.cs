using UniModules.UniGame.Core.Editor.EditorProcessors;
using UniModules.UniGame.Core.Runtime.DataStructure;
using UniGame.Core.Runtime.SerializableType;
using UniGame.Core.Runtime.SerializableType.Attributes;

namespace UniModules.GameFlow.Editor.Processor.FlowProcessors
{
    using System;
    using System.Collections.Generic;
    using Abstract;
    using global::UniModules.GameFlow.Runtime.Interfaces;
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
