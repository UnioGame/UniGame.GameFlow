namespace UniModules.UniGame.GameFlow.GameFlowEditor.Editor.UiElementsEditor.Nodes
{
    using global::UniGame.GameFlowEditor.Editor;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
    using GraphProcessor;
    using Runtime.Nodes;


    [NodeCustomEditor(typeof(PointNodeData))]
    public class PointNodeView : UniNodeView
    {
        protected override void DrawNode(INode sourceNode)
        {
        }
    }
}
