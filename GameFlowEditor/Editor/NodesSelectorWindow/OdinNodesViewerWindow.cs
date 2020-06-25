#if ODIN_INSPECTOR

namespace UniModules.UniGameFlow.GameFlowEditor.Editor.NodesSelectorWindow
{
    using System;
    using System.Collections.Generic;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Core;
    using Sirenix.OdinInspector.Editor;
    using UniGreenModules.UniGame.Core.Runtime.SerializableType;
    using UnityEditor;

    [Serializable]
    public class NodeInfoData
    {
        public string Name;
        public SType NodeType;
        public string Description;
    }
    
    public class OdinNodesViewerWindow : OdinEditorWindow
    {
        #region static initialization
        
        [MenuItem("UniGame/GameFlow/Nodes Window")]
        private static void OpenWindow()
        {
            var window = GetWindow<OdinNodesViewerWindow>();
            window.Show();
        }
        
        #endregion
        
        public string filter;
        
        public List<NodeInfoData> nodes = new List<NodeInfoData>();


        public void Initialize(INodeGraph nodeGraph)
        {
            
        }
        
        public void Reload()
        {
            
        }
        
    }
}


#endif