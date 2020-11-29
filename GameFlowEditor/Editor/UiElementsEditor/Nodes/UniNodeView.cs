using GraphProcessor;

namespace UniGame.GameFlowEditor.Editor
{
    using System.Collections.Generic;
    using Runtime;
    using UniModules.UniCore.EditorTools.Editor.Utility;
    using UniModules.UniGame.Context.Editor.ContextEditorWindow;
    using UniModules.UniGame.GameFlow.GameFlowEditor.Editor.UiElementsEditor.Processor;
    using UniModules.UniGame.GameFlow.GameFlowEditor.Editor.UiElementsEditor.Tools.PortData;
    using UniModules.UniGameFlow.GameFlowEditor.Editor.Tools;
    using UniNodes.NodeSystem.Runtime.Core;
    using UniNodes.NodeSystem.Runtime.Interfaces;
    using UnityEngine;
    using UnityEngine.UIElements;

    [NodeCustomEditor(typeof(UniBaseNode))]
    public class UniNodeView : BaseNodeView
    {
        private const string                   PortsInfoMenu    = "Ports Data Info";
        private const string                   OpenScriptMenu    = "Open UniNode Script";
        
        private       List<ContextDescription> content          = new List<ContextDescription>();
        private       Color                    _backgroundColor = new Color(0.4f, 0.4f, 0.4f);

        private SerializableNodeContainer serializableNode;

        protected SerializableNodeContainer NodeContainer
        {
            get
            {
                if (!serializableNode)
                    serializableNode = ScriptableObject.CreateInstance<SerializableNodeContainer>();
                return serializableNode;
            }
        }

        #region public properties

        public UniBaseNode NodeData { get; protected set; }

        public string Guid => NodeData.GUID;

        public int Id => NodeData.SourceNode.Id;

        #endregion

        public override void Enable()
        {
            NodeData = nodeTarget as UniBaseNode;
            var sourceNode = NodeData?.SourceNode;
            if (sourceNode is SerializableNode assetNode)
            {
                NodeContainer.Initialize(assetNode, sourceNode.GraphData as NodeGraph);
            }

            DrawNode(sourceNode);
            
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
            content.Clear();
            foreach (var nodePort in NodeData.SourceNode.Ports)
            {
                content.Add(new ContextDescription()
                {
                    Data  = nodePort.Value,
                    Label = nodePort.ItemName
                });
            }

            ContextContentWindow.Open(content).Focus();
        }

        protected virtual void DrawNode(INode sourceNode)
        {
            var container      = sourceNode.DrawNodeUiElements();
            var containerStyle = container.style;

            containerStyle.backgroundColor = new StyleColor(_backgroundColor);
            containerStyle.paddingTop      = 4;
            containerStyle.paddingLeft     = 4;
            containerStyle.marginBottom    = 4;

            containerStyle.paddingTop    = 4;
            containerStyle.paddingBottom = 4;
            containerStyle.marginLeft    = 4;
            containerStyle.marginRight   = 4;
            containerStyle.minWidth      = 250;
            
            controlsContainer.Add(container);

        }
        
        private void ShowPortsValues()
        {
            PortDataWindow.Open(NodeData.SourceNode).Focus();
        }

        private void OpenUniNodeSourceCode()
        {
            NodeData.SourceNode.GetType().OpenEditorScript();
        }

        protected void OnDestroy()
        {
            NodeViewProcessor.Asset.Remove(this);
        }
    }
}