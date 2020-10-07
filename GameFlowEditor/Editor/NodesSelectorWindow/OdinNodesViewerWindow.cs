#if ODIN_INSPECTOR

namespace UniModules.UniGameFlow.GameFlowEditor.Editor.NodesSelectorWindow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Attributes;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Core;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
    using Sirenix.OdinInspector.Editor;
    using UniGreenModules.UniCore.Runtime.ReflectionUtils;
    using UniGreenModules.UniCore.Runtime.Utils;
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

        private static Type                           nodeBaseType = typeof(INode);
        private static MemorizeItem<Type, List<Type>> nodeTypes    = MemorizeTool.
            Memorize<Type, List<Type>>(x => {
                var items = nodeBaseType.GetAssignableTypes().
                    Where(node => !node.HasAttribute<HideNodeAttribute>()).
                    ToList();
            
                return items;
            });
        
        [MenuItem("UniGame/GameFlow/Nodes Search Window")]
        private static void OpenWindow()
        {
            var window = GetWindow<OdinNodesViewerWindow>();
            window.InitializeWindow();
            window.Show();
        }
        
        #endregion
        
        public string filter;
        
        public List<NodeInfoData> nodes = new List<NodeInfoData>();

        public void InitializeWindow()
        {
            Reload();
        }
        
        [Sirenix.OdinInspector.Button]
        public void Reload()
        {
            RefreshNodeList();
        }

        private void RefreshNodeList()
        {
            nodes.Clear();
            var nodeItems = nodeTypes[nodeBaseType];
            
            foreach (var item in nodeItems) {

                var infoData = item.GetCustomAttribute<NodeInfoAttribute>();

                var itemInfo = new NodeInfoData() {
                    Description = "Info",
                    Name = item.Name,
                    NodeType = item
                };
                
            }
        }
        
    }
}


#endif