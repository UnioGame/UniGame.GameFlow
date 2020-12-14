using System;

namespace UniGame.GameFlowEditor.Editor
{
    using System.Collections.Generic;
    using System.Linq;
    using GraphProcessor;
    using Runtime;
    using UniModules.UniCore.EditorTools.Editor.PrefabTools;
    using UniModules.UniCore.EditorTools.Editor.Utility;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.GameFlow.GameFlowEditor.Editor.UiElementsEditor;
    using UniModules.UniGame.GameFlow.GameFlowEditor.Editor.UiElementsEditor.Tools.PortData;
    using UniModules.UniGameFlow.GameFlowEditor.Editor.NodesSelectorWindow;
    using UniModules.UniGameFlow.GameFlowEditor.Editor.Tools;
    using UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor;
    using UniNodes.NodeSystem.Runtime.Core;
    using UniNodes.NodeSystem.Runtime.Core.Nodes;
    using UniNodes.NodeSystem.Runtime.Interfaces;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    [Serializable]
    public class GameFlowGraphView : 
        BaseGraphView, IGameFlowGraphView
    {
        private const    string                           NodesMenu      = "UniNodes";
        private const    string                           NodesInfoWindowMenu      = "Nodes Window";
        
        private readonly LifeTimeDefinition               lifeTimeDefinition = new LifeTimeDefinition();
        private readonly Dictionary<BaseNode,UniNodeView> registeredNodes    = new Dictionary<BaseNode,UniNodeView>(16);
        private          bool                             selectionUpdated   = false;
        
        private SerializableNodeContainer selectedNode;
        protected SerializableNodeContainer SelectionContainer {
            get {
                if (!selectedNode)
                    selectedNode = ScriptableObject.
                        CreateInstance<SerializableNodeContainer>();
                return selectedNode;
            }
        }
        
        #region constructor
        
        public GameFlowGraphView(UniGameFlowWindow window) : base(window)
        {
            GameFlowWindow = window;
        }
        
        #endregion
        
        #region public properties

        public UniGameFlowWindow GameFlowWindow { get; private set; }

        public IUniGraph ActiveGraph => SourceGraph.UniGraph;

        public UniAssetGraph SourceGraph { get; protected set; }

        public ILifeTime LifeTime => lifeTimeDefinition;

        public Vector2 LastMenuPosition { get; protected set; }

        #endregion

        public void Focus(INode node)
        {
            var selection = node is Object asset ? 
                asset : 
                SelectionContainer.Initialize(node as SerializableNode, node.GraphData as NodeGraph);
            selection.AddToEditorSelection(false);
        }

        public void Save()
        {
            var graphData = SourceGraph.UniGraph;

            graphData.SetPosition(graph.position);
            
            SaveGroupInfo(graphData);
            SaveStackNodeInfo(graphData);

            UpdateNodePositions();
            //save prefab data
            SourceGraph.UniGraph.MarkDirty();
            SourceGraph.UniGraph.Save();
        }

        #region graph api

        public List<int> GetNodesIdsByGuid(List<string> guids)
        {
            var ids = nodeViews.
                OfType<UniNodeView>().
                Where(x => guids.Contains(x.Guid)).
                Select(x => x.Id).
                ToList();
            
            return ids;
        }
        
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendSeparator();
            
            var mousePos = (evt.currentTarget as VisualElement).
                ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            var nodePosition = mousePos;
            
            LastMenuPosition = nodePosition;

            AddNodesMenus(evt, nodePosition);
            AddNodesWindowMenu(evt);
            
            evt.menu.AppendSeparator();
            
            base.BuildContextualMenu(evt);
        }

        
        
        #endregion

        #region private methods

        private void AddNodesMenus(ContextualMenuPopulateEvent evt,Vector3 nodePosition)
        {
            foreach (var nodeType in NodeEditorUtilities.NodeTypes)
            {
                var menuName = $"{NodesMenu}/{nodeType.GetNodeMenuName()}";
                evt.menu.AppendAction(menuName,
                    (e) => {
                        var node = SourceGraph.CreateNode(nodeType, nodePosition);
                    },
                    DropdownMenuAction.AlwaysEnabled
                );
            }
        }

        private void AddNodesWindowMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction(NodesInfoWindowMenu,
                (e) => NodesInfoWindow.ShowWindow(),
                DropdownMenuAction.AlwaysEnabled
            );
        }

        
        protected override void InitializeView()
        {
            lifeTimeDefinition.Release();
            
            SourceGraph = graph as UniAssetGraph;

            BindEvents();
            CreateGroupViews(SourceGraph.UniGraph);
            
            lifeTimeDefinition.AddCleanUpAction(() => registeredNodes.Clear());
        }

        
        private void BindEvents()
        {
            RegisterCallback<AttachToPanelEvent>(_ => UpdateGraph());
            RegisterCallback<DetachFromPanelEvent>(_ => UpdateGraph());
            
            SourceGraph.onGraphChanges += OnOnGraphChanges;
        }

        private void UpdateGraph()
        {
            foreach (var nodePair in nodeViewsPerNode) {

                switch (nodePair.Value) {
                    case UniNodeView nodeView:
                        UpdateUniNode(nodeView,nodePair.Key);
                        break;
                    //Todo some other behaviours
                }
                
            }
            
            selectionUpdated = false;
        }

        private void CreateGroupViews(UniGraph graphData)
        {
            graphData.nodeGroups.RemoveAll(x => x == null);
            foreach (var group in graphData.nodeGroups) {
                var title = string.IsNullOrEmpty(group.Title) ? string.Empty : group.Title;
                var groupData = new Group(title,group.Size) {
                    color = group.Color,
                    position = group.Position,
                    innerNodeGUIDs = nodeViews.
                        OfType<UniNodeView>().
                        Where(x => group.NodeIds.Contains(x.Id)).
                        Select(x => x.Guid).
                        ToList()
                };
                AddGroup(groupData);
            }
        }
        
        private void SaveStackNodeInfo(UniGraph graphData)
        {
            foreach (var groupView in stackNodeViews) {
                
            }
        }
        
        private void SaveGroupInfo(UniGraph graphData)
        {
            graphData.nodeGroups.Clear();

            foreach (var groupView in groupViews) {
                var groupData = groupView.group;
                var groupInfo = new NodesGroup() {
                    color    = groupData.color,
                    position = groupData.position,
                    size     = groupData.size,
                    title    = groupData.title,
                    nodeIds  = GetNodesIdsByGuid(groupData.innerNodeGUIDs),
                };
                graphData.nodeGroups.Add(groupInfo);
            }
        }
        
        private bool UpdateSelection(UniNodeView nodeView,UniBaseNode nodeData)
        {
            if (selectionUpdated || !nodeView.selected) 
                return selectionUpdated;
            
            var sourceNode = nodeData.SourceNode;
            Focus(sourceNode);

            return true;
        }

        private void CheckNodeInitialization(UniNodeView nodeView,UniBaseNode nodeData)
        {
            if (registeredNodes.TryGetValue(nodeData, out var view))
                return;

            registeredNodes[nodeData] = nodeView;
            
            nodeView.RegisterCallback<MouseDownEvent>(_ => UpdateSelection(nodeView,nodeData));
        }
        
        private void UpdateUniNode(UniNodeView nodeView,BaseNode nodeData)
        {
            if (!(nodeData is UniBaseNode uniNode))
                return;
            
            CheckNodeInitialization(nodeView,uniNode);
            selectionUpdated = UpdateSelection(nodeView,uniNode);
        }
        
        private void OnOnGraphChanges(GraphChanges changes)
        {
            switch (changes) {
                case GraphChanges _ when changes.addedNode != null :
                    UniNodeAction(changes.addedNode,OnNodeAdded);
                    break;
                case GraphChanges _ when changes.removedNode != null :
                    UniNodeAction(changes.removedNode,OnNodeRemoved);
                    break;
                case GraphChanges _ when changes.removedEdge != null :
                    UniPortAction(changes.removedEdge,OnEdgeRemoved);
                    break;
                case GraphChanges _ when changes.addedEdge != null :
                    UniPortAction(changes.addedEdge,OnEdgeAdded);
                    break;
                case GraphChanges _ when changes.addedGroups != null :
                    break;
                case GraphChanges _ when changes.removedGroups != null :
                    break;
                case GraphChanges _ when changes.removedStackNode != null :
                    break;
                case GraphChanges _ when changes.addedStackNode != null :
                    break;
                case GraphChanges _ when changes.nodeChanged != null :
                    UniNodeAction(changes.nodeChanged,OnNodeChanged);
                    break;
            }
        }

        
        private void UniNodeAction(BaseNode node,Action<UniBaseNode> action)
        {
            if (node is UniBaseNode uniNode)
                action(uniNode);
        }

        private void UniPortAction(SerializableEdge edge, Action<INodePort, INodePort> portAction)
        {
            if(!(edge.outputNode is UniBaseNode outputNode))
                return;
            if(!(edge.inputNode is UniBaseNode inputNode))
                return;

            var inputPort  = edge.inputPort;
            var outputPort = edge.outputPort;

            var inputPortName = inputPort.portData.identifier;
            var outputPortName = outputPort.portData.identifier;

            var output = outputNode.SourceNode;
            var input  = inputNode.SourceNode;

            var fromPort = output.GetPort(outputPortName);
            var toPort   = input.GetPort(inputPortName);
            
            portAction(fromPort, toPort);
            
        }
        
        private void OnEdgeAdded(INodePort fromPort, INodePort toPort)
        {
            fromPort.Connect(toPort);
        }
        
        private void OnEdgeRemoved(INodePort fromPort, INodePort toPort)
        {
            fromPort.Disconnect(toPort);
        }

        private void OnNodeAdded(UniBaseNode node)
        {
            AddNodeView(node);
        }
        
        private void OnNodeRemoved(UniBaseNode node)
        {
            SourceGraph.UniGraph.
                RemoveNode(node.SourceNode);
        }

        private void OnNodeChanged(UniBaseNode node)
        {

        }

        private void UpdateNodePositions()
        {
            var viewNodes = nodeViews.OfType<UniNodeView>().ToList();
            foreach (var nodeView in viewNodes)
            {
                //package view position calculation bug
                var position = nodeView.GetPosition().position;
                if (position == Vector2.zero)
                    return;
                nodeView.NodeData.SourceNode.Position = position;
            }
        }
        
        #endregion
    }
}
