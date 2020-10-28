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
    using global::UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor;
    using UnityEngine;

    public enum NodeSortingType {
        Name,
        MenuName,
        Category
    }
    
    [Serializable]
    public class NodesViewerEditor {
        
        #region static data
        
        private static Type nodeBaseType = typeof(INode);
        private static MemorizeItem<Type, List<Type>> nodeTypes = MemorizeTool.Memorize<Type, List<Type>>(x => {
            var items = nodeBaseType.GetAssignableTypes().
                Where(node => !node.HasAttribute<HideNodeAttribute>()).
                ToList();
            return items;
        });
        
        #endregion

        public NodeSortingType sortBy = NodeSortingType.MenuName;
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Searchable(FilterOptions = Sirenix.OdinInspector.SearchFilterOptions.ISearchFilterableInterface)]
        [Sirenix.OdinInspector.InlineProperty]
        [Sirenix.OdinInspector.HideLabel]
        [Sirenix.OdinInspector.ListDrawerSettings(
            HideAddButton               = true,
            HideRemoveButton            = true,
            Expanded                    = true,
            OnEndListElementGUI = nameof(EndOfListItemGui),
            CustomRemoveElementFunction = nameof(OnSelectionAction),
            CustomRemoveIndexFunction   = nameof(OnSelectionIndexAction))]
#endif
        public List<NodeInfoData> nodes = new List<NodeInfoData>();

        public void Reload() {
            nodes = RefreshNodeList();
        }

        #region inspector

        private List<NodeInfoData> RefreshNodeList() {
            var nodeDatas = CreateNodesInfo();
            nodeDatas = FilterNodeInfo(nodeDatas);
            
            return nodeDatas;
        }

        private void OnSelectionAction(NodeInfoData data) {
            GUILayout.Button("Select");
        }

        private void OnSelectionIndexAction(int index) {
            
        }

        private List<NodeInfoData> CreateNodesInfo() {
            var nodeDatas = new List<NodeInfoData>();
            var nodeItems = nodeTypes[nodeBaseType];
            

            foreach (var item in nodeItems) {
                nodeDatas.Add(CreateInfo(item));
            }

            return nodeDatas;
        }
        
        private List<NodeInfoData> FilterNodeInfo(List<NodeInfoData> nodesInfo) {

            switch (sortBy) {
                case NodeSortingType.Name:
                    nodesInfo = nodesInfo.OrderBy(x => x.Name).ToList();
                    break;
                case NodeSortingType.MenuName:
                    nodesInfo = nodesInfo.OrderBy(x => x.MenuName).ToList();
                    break;
                case NodeSortingType.Category:
                    nodesInfo = nodesInfo.OrderBy(x => x.Category).ToList();
                    break;
            }
            
            return nodesInfo;
        }
        
        private void EndOfListItemGui(int item)
        {
            if (GUILayout.Button("add to graph")) {
                
            }
        }
        
        private NodeInfoData CreateInfo(Type nodeType) {
            
            var nodeInfo = nodeType.GetCustomAttribute<INodeInfo>();

            var itemInfo = new NodeInfoData() {
                Script      = nodeType.GetScriptAsset(),
                Description = string.Empty,
                Category = string.Empty,
                Name        = nodeType.Name,
                MenuName = nodeType.GetNodeMenuName()
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