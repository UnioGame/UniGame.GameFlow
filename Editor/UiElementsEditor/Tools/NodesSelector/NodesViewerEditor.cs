#if ODIN_INSPECTOR

namespace UniModules.UniGame.GameFlow.GameFlowEditor.Editor.NodesSelectorWindow.OdinWindow {
    using System.Collections.Generic;
    using System.Linq;
    using UniModules.Editor;
    using global::UniModules.GameFlow.Runtime.Attributes;
    using global::UniModules.GameFlow.Runtime.Interfaces;
    using UniCore.Runtime.ReflectionUtils;
    using UniCore.Runtime.Utils;
    using System;
    using global::UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor;
    using UniModules.GameFlow.Editor;
    using UnityEditor;
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

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.OnValueChanged(nameof(OnSearchFilterChanged))]
#endif
        public string search = string.Empty;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.OnValueChanged(nameof(OnSearchFilterChanged))]
#endif
        public NodeSortingType sortBy = NodeSortingType.MenuName;
        
        [HideInInspector]
        public List<NodeInfoData> sourceNodes = new List<NodeInfoData>();
        
#if ODIN_INSPECTOR
// #if ODIN_INSPECTOR_3
//         [Sirenix.OdinInspector.Searchable(FilterOptions = Sirenix.OdinInspector.SearchFilterOptions.ISearchFilterableInterface)]
// #endif
        [Sirenix.OdinInspector.InlineProperty]
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
            sourceNodes = RefreshNodeList();
            OnSearchFilterChanged();
        }

        #region inspector

        private List<NodeInfoData> RefreshNodeList() {
            var nodeDatas = CreateNodesInfo();
            return nodeDatas;
        }

        private void OnSearchFilterChanged()
        {
            var nodeDatas = FilterNodeInfo(sourceNodes);
            nodeDatas = nodeDatas.Where(x => x.IsMatch(search));
            nodes.Clear();
            nodes.AddRange(nodeDatas);
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
        
        private IEnumerable<NodeInfoData> FilterNodeInfo(List<NodeInfoData> nodesInfo) {

            switch (sortBy) {
                case NodeSortingType.Name:
                    return nodesInfo.OrderBy(x => x.Name);
                case NodeSortingType.MenuName:
                    return nodesInfo.OrderBy(x => x.MenuName);
                case NodeSortingType.Category:
                    return nodesInfo.OrderBy(x => x.Category);
            }
            
            return Enumerable.Empty<NodeInfoData>();
        }

        private void EndOfListItemGui(int item)
        {
            var focused    = UniGameFlowWindow.FocusedWindow;
            var nodeItem   = nodes[item];
            
            var isDisabled = !focused || focused.IsEmpty || !focused.IsVisible;
            
            EditorDrawerUtils.DrawDisabled(() =>
            {
                var graphName = isDisabled ? "none" : focused.GraphName;
                if (!GUILayout.Button($"add to graph [{graphName}]"))
                    return;
                
                focused.AddNode(nodeItem.NodeType,nodeItem.Name);
                focused.Focus();
                
            },isDisabled);
        }
        
        private NodeInfoData CreateInfo(Type nodeType) {
            
            var nodeInfo = nodeType.GetCustomAttribute<INodeInfo>();

            var itemInfo = new NodeInfoData() {
                Script      = nodeType.GetScriptAsset(),
                Description = string.Empty,
                Category = string.Empty,
                Name        = nodeType.Name,
                NodeType =  nodeType,
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