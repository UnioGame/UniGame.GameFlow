using GraphProcessor;

namespace UniGame.GameFlowEditor.Editor
{
    using System.Collections.Generic;
    using Core.EditorTools.Editor.UiElements;
    using Runtime;
    using UniGreenModules.UniCore.EditorTools.Editor.Utility;
    using UniNodes.NodeSystem.Inspector.Editor.ContentContextWindow;
    using UniNodes.NodeSystem.Runtime.Core;
    using UniNodes.NodeSystem.Runtime.Core.Nodes;
    using UnityEngine;
    using UnityEngine.UIElements;


    [NodeCustomEditor(typeof(UniBaseNode))]
    public class UniNodeView : BaseNodeView
    {
        private List<ContextDescription> content = new List<ContextDescription>();
        
        private SerializableNodeContainer serializableNode;
        protected SerializableNodeContainer NodeContainer {
            get {
                if (!serializableNode)
                    serializableNode = ScriptableObject.CreateInstance<SerializableNodeContainer>();
                return serializableNode;
            }
        }
        
        #region public properties
        
        public UniBaseNode Context { get; protected set; }

        public string Guid => Context.GUID;

        public int Id => Context.SourceNode.Id;

        #endregion
        
        public override void Enable()
        {
            Context = nodeTarget as UniBaseNode;
            var sourceNode = Context?.SourceNode;

            if (!(sourceNode is Object assetNode)) {
                NodeContainer.Initialize(sourceNode as SerializableNode);
            }
            
            var container = UiElementFactory.Create(sourceNode);
            var containerStyle = container.style;
            
            containerStyle.paddingTop = 4;
            containerStyle.paddingBottom = 4;
            containerStyle.marginLeft = 4;
            containerStyle.marginRight = 4;
            containerStyle.minWidth = 220;

            controlsContainer.Add(container);
        }

        
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Open UniNode Script", (e) => OpenUniNodeSourceCode());
            evt.menu.AppendAction("Show UniNode Ports Data", (e) => ShowPortsData());
            base.BuildContextualMenu(evt);
        }

        public void ShowPortsData()
        {
            content.Clear();
            foreach (var nodePort in Context.SourceNode.Ports) {
                content.Add(new ContextDescription() {
                    Data = nodePort.Value,
                    Label = nodePort.ItemName
                });
            }
            ContextContentWindow.Open(content);
        }
        
        private void OpenUniNodeSourceCode()
        {
            Context.SourceNode.
                GetType().
                OpenEditorScript();
        }

    }
}
