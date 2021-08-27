using UniGame.GameFlowEditor.Editor;
using UniModules.GameFlow.Runtime.Interfaces;
using UniModules.UniGame.GameFlow.GameFlowEditor.Runtime.Nodes;

namespace UniModules.GameFlow.Editor.Nodes
{
    using GraphProcessor;


    [NodeCustomEditor(typeof(PointNodeData))]
    public class PointNodeView : UniNodeView
    {
        protected override void DrawNode(INode sourceNode)
        {
        }
    }
}
