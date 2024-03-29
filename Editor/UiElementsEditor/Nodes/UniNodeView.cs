﻿using GraphProcessor;
using UniModules.UniCore.Runtime.DataFlow;
using UniRx;

namespace UniGame.GameFlowEditor.Editor
{
    using System.Collections.Generic;
    using Runtime;
    using UniModules.Editor;
    using UniModules.UniGame.Context.Editor.ContextEditorWindow;
    using UniModules.GameFlow.Editor.Processor;
    using UniModules.GameFlow.Editor.Tools.PortData;
    using UniModules.UniGameFlow.GameFlowEditor.Editor.Tools;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    [NodeCustomEditor(typeof(UniBaseNode))]
    public class UniNodeView : BaseNodeView
    {
        private const string PortsInfoMenu = "Ports Data Info";
        private const string OpenScriptMenu = "Open UniNode Script";
        private const string BaseContainerStyle = "uninode-view-container";

        private List<ContextDescription> _content = new List<ContextDescription>();
        private Color _backgroundColor = new Color(0.4f, 0.4f, 0.4f);
        private SerializableNodeContainer _serializableNode;
        private LifeTimeDefinition _lifeTime = new LifeTimeDefinition();
        private INode node;

        private SerializableNodeContainer NodeContainer
            => _serializableNode ??= ScriptableObject.CreateInstance<SerializableNodeContainer>();


        #region public properties
        
        public UniBaseNode NodeData { get; protected set; }

        public string Guid => NodeData.GUID;

        public int Id => NodeData.SourceNode.Id;

        public bool IsSerializable { get; protected set; }

        #endregion

        public override void Enable()
        {
            NodeData = nodeTarget as UniBaseNode;

            var sourceNode = NodeData?.SourceNode;

            if (sourceNode is SerializableNode assetNode)
            {
                IsSerializable = true;
                NodeContainer.Initialize(assetNode, sourceNode.GraphData as NodeGraph);
            }

            NodeData?.SourceNodeObservable
                .Where(x => node != x)
                .Do(ApplyNodeInfo)
                .Do(DrawNode)
                .Do(x => ForceUpdatePorts())
                .Subscribe()
                .AddTo(_lifeTime);
            
            //add into node processor
            NodeViewProcessor.Asset.Add(this);
        }


        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction(OpenScriptMenu, (e) => OpenUniNodeSourceCode());
            evt.menu.AppendAction(PortsInfoMenu, (e) => ShowPortsValues());
            base.BuildContextualMenu(evt);
        }

        public void ShowPortsData()
        {
            _content.Clear();
            foreach (var nodePort in NodeData.SourceNode.Ports)
            {
                _content.Add(new ContextDescription()
                {
                    Data = nodePort.Value,
                    Label = nodePort.ItemName
                });
            }

            ContextContentWindow.Open(_content).Focus();
        }

        protected virtual void DrawNode(INode sourceNode)
        {
            if (sourceNode == null) return;

            var container = this.Q<VisualElement>(BaseContainerStyle, BaseContainerStyle);
            if (container != null)
                container.visible = false;

            container = node.DrawNodeUiElements();
            container.name = BaseContainerStyle;
            container.AddToClassList(BaseContainerStyle);

            controlsContainer.Add(container);

            ApplyStyle(container);
        }

        protected void ApplyNodeInfo(INode sourceNode)
        {
            node = sourceNode;
            title = node.ItemName;
        }

        public override void OnSelected()
        {
            base.OnSelected();
            Selection.activeObject = IsSerializable ? NodeContainer : NodeData.SourceNode as Object;
        }

        protected virtual void ApplyStyle(VisualElement container)
        {
            var containerStyle = container.style;

            containerStyle.backgroundColor = new StyleColor(_backgroundColor);
            containerStyle.paddingTop = 4;
            containerStyle.paddingLeft = 4;
            containerStyle.marginBottom = 4;

            containerStyle.paddingTop = 4;
            containerStyle.paddingBottom = 4;
            containerStyle.marginLeft = 4;
            containerStyle.marginRight = 4;
            containerStyle.minWidth = 250;

        }

        private void ShowPortsValues()
        {
            PortDataWindow.Open(NodeData.SourceNode).Focus();
        }

        private void OpenUniNodeSourceCode() => NodeData.SourceNode.GetType().OpenEditorScript();

        protected void OnDestroy()
        {
            _lifeTime.Terminate();
            NodeViewProcessor.Asset.Remove(this);
        }
    }
}