using System;
using UniGame.Tools;
using UniModules.GameFlow.Runtime.Attributes;
using UniModules.UniCore.Runtime.ReflectionUtils;

namespace UniGame.GameFlowEditor.Editor
{
    using System.Collections.Generic;
    using System.Linq;
    using GraphProcessor;
    using Runtime;
    using UniModules.Editor;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.GameFlow.Editor;
    using UniModules.UniGameFlow.GameFlowEditor.Editor.NodesSelectorWindow;
    using UniModules.UniGameFlow.GameFlowEditor.Editor.Tools;
    using UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UnityEngine;
    using UnityEngine.UIElements;

    [Serializable]
    public class GameFlowGraphView : BaseGraphView, IGameFlowGraphView
    {
        private const string NodesMenu           = "UniNodes";
        private const string NodesInfoWindowMenu = "Nodes Window";

        private readonly LifeTimeDefinition                _lifeTimeDefinition = new LifeTimeDefinition();
        private readonly Dictionary<BaseNode, UniNodeView> _registeredNodes    = new Dictionary<BaseNode, UniNodeView>(16);
        private          bool                              _selectionUpdated   = false;

        private SerializableNodeContainer _selectedNode;

        protected SerializableNodeContainer SelectionContainer
        {
            get
            {
                if (!_selectedNode)
                    _selectedNode = ScriptableObject.CreateInstance<SerializableNodeContainer>();
                return _selectedNode;
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

        public UniGraphAsset SourceGraph { get; protected set; }

        public ILifeTime LifeTime => _lifeTimeDefinition;

        public Vector2 LastMenuPosition { get; protected set; }

        #endregion

        public void Focus(INode node)
        {
            var selectedAsset = node is Object asset
                ? asset
                : SelectionContainer.Initialize(node as SerializableNode, node.GraphData as NodeGraph);
            selectedAsset.AddToEditorSelection(false);
        }

        public void Save()
        {
            var graphData = SourceGraph.UniGraph;
            graphData.SetPosition(graph.position);
            UpdateNodePositions();

            if (!graphData) return;
            graphData.MarkDirty();
            graphData.serializedGraph.MarkDirty();
            //save prefab data
            graphData.Save();
        }

        #region graph api

        public List<int> GetNodesIdsByGuid(List<string> guids)
        {
            var ids = nodeViews.OfType<UniNodeView>()
                .Where(x => guids.Contains(x.Guid))
                .Select(x => x.Id)
                .ToList();

            return ids;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendSeparator();

            var mousePos     = (evt.currentTarget as VisualElement)
                .ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            var nodePosition = mousePos;

            LastMenuPosition = nodePosition;

            AddNodesMenus(evt, nodePosition);
            AddNodesWindowMenu(evt);

            evt.menu.AppendSeparator();

            base.BuildContextualMenu(evt);
        }

        #endregion

        #region private methods

        private void AddNodesMenus(ContextualMenuPopulateEvent evt, Vector3 nodePosition)
        {
            foreach (var nodeType in NodeEditorUtilities.NodeTypes)
            {
                var isHidden = nodeType.HasAttribute(typeof(HideNodeAttribute)) || nodeType.IsGenericType;
                if(isHidden) continue;
                
                var menuName = $"{NodesMenu}/{nodeType.GetNodeMenuName()}";
                evt.menu.AppendAction(menuName, (e) => SourceGraph.CreateNode(nodeType, nodePosition),
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
            _lifeTimeDefinition.Release();

            SourceGraph = graph as UniGraphAsset;

            BindEvents();

            _lifeTimeDefinition.AddCleanUpAction(_registeredNodes.Clear);
        }


        private void BindEvents()
        {
            RegisterCallback<AttachToPanelEvent>(_ => UpdateGraph());
            RegisterCallback<DetachFromPanelEvent>(_ => UpdateGraph());

            SourceGraph.onGraphChanges += OnOnGraphChanges;
        }

        private void UpdateGraph()
        {
            foreach (var nodePair in nodeViewsPerNode)
            {
                switch (nodePair.Value)
                {
                    case UniNodeView nodeView:
                        UpdateUniNode(nodeView, nodePair.Key);
                        break;
                    //Todo some other behaviours
                }
            }

            _selectionUpdated = false;
        }

        private bool UpdateSelection(UniNodeView nodeView, UniBaseNode nodeData)
        {
            if (_selectionUpdated || !nodeView.selected)
                return _selectionUpdated;

            var sourceNode = nodeData.SourceNode;
            Focus(sourceNode);

            return true;
        }

        private void CheckNodeInitialization(UniNodeView nodeView, UniBaseNode nodeData)
        {
            if (_registeredNodes.TryGetValue(nodeData, out var view))
                return;

            _registeredNodes[nodeData] = nodeView;

            nodeView.RegisterCallback<MouseDownEvent>(_ => UpdateSelection(nodeView, nodeData));
        }

        private void UpdateUniNode(UniNodeView nodeView, BaseNode nodeData)
        {
            if (!(nodeData is UniBaseNode uniNode))
                return;

            CheckNodeInitialization(nodeView, uniNode);
            _selectionUpdated = UpdateSelection(nodeView, uniNode);
        }

        private void OnOnGraphChanges(GraphChanges changes)
        {
            switch (changes)
            {
                case { addedNode: { } }:
                    UniNodeAction(changes.addedNode, OnNodeAdded);
                    break;
                case { removedNode: { } }:
                    UniNodeAction(changes.removedNode, OnNodeRemoved);
                    break;
                case { removedEdge: { } }:
                    UniPortAction(changes.removedEdge, OnEdgeRemoved);
                    break;
                case { addedEdge: { } }:
                    UniPortAction(changes.addedEdge, OnEdgeAdded);
                    break;
                case { addedGroups: { } }:
                    break;
                case { removedGroups: { } }:
                    break;
                case { removedStackNode: { } }:
                    break;
                case { addedStackNode: { } }:
                    break;
                case { nodeChanged: { } }:
                    UniNodeAction(changes.nodeChanged, OnNodeChanged);
                    break;
            }

            var changedGraph = SourceGraph.UniGraph;
            if (!changedGraph) return;
            changedGraph.MarkDirty();
            changedGraph.serializedGraph.MarkDirty();
        }


        private void UniNodeAction(BaseNode node, Action<UniBaseNode> action)
        {
            if (node is UniBaseNode uniNode)
                action(uniNode);
        }

        private void UniPortAction(SerializableEdge edge, Action<INodePort, INodePort> portAction)
        {
            if (!(edge.outputNode is UniBaseNode outputNode))
                return;
            if (!(edge.inputNode is UniBaseNode inputNode))
                return;

            var inputPort  = edge.inputPort;
            var outputPort = edge.outputPort;

            var inputPortName  = inputPort.portData.identifier;
            var outputPortName = outputPort.portData.identifier;

            var output = outputNode.SourceNode;
            var input  = inputNode.SourceNode;

            var fromPort = output.GetPort(outputPortName);
            var toPort   = input.GetPort(inputPortName);

            portAction(fromPort, toPort);
        }

        private void OnEdgeAdded(INodePort fromPort, INodePort toPort) => fromPort.Connect(toPort);

        private void OnEdgeRemoved(INodePort fromPort, INodePort toPort) => fromPort.Disconnect(toPort);

        private void OnNodeAdded(UniBaseNode node) => AddNodeView(node);

        private void OnNodeRemoved(UniBaseNode node)
        {
            if (node.SourceNode == null) return;
            SourceGraph.UniGraph.RemoveNode(node.SourceNode);
        }

        private void OnNodeChanged(UniBaseNode node)
        {
        }

        private void UpdateNodePositions()
        {
            var viewNodes = nodeViews
                .OfType<UniNodeView>()
                .ToList();
            
            foreach (var nodeView in viewNodes)
            {
                var sourceNode = nodeView.NodeData.SourceNode;
                if (sourceNode == null)
                    continue;
                
                //package view position calculation bug
                var position = nodeView.GetPosition().position;
                if (position == Vector2.zero)
                    return;

                sourceNode.Position = position;
            }
        }

        #endregion
    }
}