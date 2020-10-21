#if ODIN_INSPECTOR

namespace UniModules.UniGame.GameFlow.GameFlowEditor.Editor.NodesSelectorWindow.OdinWindow {
    using System.Collections.Generic;
    using System.Linq;
    using Core.EditorTools.Editor.AssetOperations;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Attributes;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
    using UniCore.Runtime.ReflectionUtils;
    using UniCore.Runtime.Utils;
    using System;
    using UnityEngine;

    [Serializable]
    public class NodesViewerEditor {
        private static Type nodeBaseType = typeof(INode);
        private static MemorizeItem<Type, List<Type>> nodeTypes = MemorizeTool.Memorize<Type, List<Type>>(x => {
            var items = nodeBaseType.GetAssignableTypes().
                Where(node => !node.HasAttribute<HideNodeAttribute>()).
                ToList();
            return items;
        });

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Searchable(FilterOptions = Sirenix.OdinInspector.SearchFilterOptions.ISearchFilterableInterface)]
        [Sirenix.OdinInspector.InlineProperty]
        [Sirenix.OdinInspector.HideLabel]
        [Sirenix.OdinInspector.ListDrawerSettings(
            HideAddButton               = true,
            HideRemoveButton            = true,
            Expanded                    = true,
            CustomRemoveElementFunction = nameof(OnSelectionAction),
            CustomRemoveIndexFunction   = nameof(OnSelectionIndexAction))]
#endif
        public List<NodeInfoData> nodes = new List<NodeInfoData>();

        public void Reload() {
            nodes = RefreshNodeList();
        }

        #region inspector

        private List<NodeInfoData> RefreshNodeList() {
            var nodeData  = new List<NodeInfoData>();
            var nodeItems = nodeTypes[nodeBaseType];
            

            foreach (var item in nodeItems) {
                nodeData.Add(CreateInfo(item));
            }

            return nodeData;
        }

        private void OnSelectionAction(NodeInfoData data) {
            GUILayout.Button("Select");
        }

        private void OnSelectionIndexAction(int index) {
            
        }
        
        private NodeInfoData CreateInfo(Type nodeType) {
            
            var nodeInfo = nodeType.GetCustomAttribute<INodeInfo>();

            var itemInfo = new NodeInfoData() {
                Script      = nodeType.GetScriptAsset(),
                Description = string.Empty,
                Category = string.Empty,
                Name        = nodeType.Name,
            };

            itemInfo.Description = nodeInfo == null || string.IsNullOrEmpty(nodeInfo.Description)
                ? itemInfo.Description
                : nodeInfo.Description;

            itemInfo.Category = nodeInfo == null || string.IsNullOrEmpty(nodeInfo.Category)
                ? itemInfo.Category
                : nodeInfo.Category;
            
            itemInfo.Name = nodeInfo == null || string.IsNullOrEmpty(nodeInfo.Name)
                ? itemInfo.Name
                : nodeInfo.Name;

            return itemInfo;
        }
        
        #endregion
    }
}

#endif