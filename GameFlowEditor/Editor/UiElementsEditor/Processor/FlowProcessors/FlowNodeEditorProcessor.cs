namespace UniModules.UniGame.GameFlow.GameFlowEditor.Editor.UiElementsEditor.Processor.NodeProcessors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abstract;
    using Core.Editor.EditorProcessors;
    using global::UniGame.GameFlowEditor.Editor;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
    using GraphProcessor;
    using UiToolkit.Runtime.Extensions;
    using UniGameFlow.GameFlowEditor.Editor.UiElementsEditor.Styles;
    using UnityEngine.UIElements;

    [Serializable]
    public class FlowNodeEditorProcessor : BaseEditorProcessorAsset<UniGameFlowWindow>,IGameFlowGraphProcessor
    {
        public StyleSheet styleSheet;
        
        
        public override void Proceed(IReadOnlyList<UniGameFlowWindow> data)
        {
            foreach (var window in data)
            {
                var root = window.rootVisualElement;
                root.Query<UniNodeView>().
                    Build().
                    ForEach(UpdateNodeView);
            }
        }

        public void UpdateNodeView(UniNodeView view)
        {
            view.AddStyleSheet(styleSheet);
            
            UpdateActiveAction(view);
            UpdateActivePorts(view);
        }

        private void UpdateActiveAction(UniNodeView view)
        {
            var node     = view.NodeData.SourceNode;
            var isActive = node.Inputs.Any(x => x.Value.HasValue);
            view.EnableInClassList(GameFlowStyleConstants.nodeActive,isActive);
        }

        private void UpdateActivePorts(UniNodeView view)
        {
            var node = view.NodeData.SourceNode;
            UpdatePortView(node, view.inputPortViews, GameFlowStyleConstants.inputPortActive);
            UpdatePortView(node, view.outputPortViews, GameFlowStyleConstants.outputPortActive);
        }

        private void UpdatePortView(INode node,List<PortView> ports,string className)
        {
            foreach (var portView in ports)
            {
                var port = node.GetPort(portView.portName);
                portView.EnableInClassList(className,port.Value.HasValue);
            }
        }
    }
}