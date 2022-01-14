using UnityEngine;
using UnityEngine.UIElements;

namespace UniGame.GameFlowEditor.Editor
{
    using GraphProcessor;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UniModules.UniGame.GameFlow.GameFlowEditor.Runtime.Nodes;

    [NodeCustomEditor(typeof(UniParameterNode))]
    public class ParameterNodeView : UniNodeView
    {
        protected override void DrawNode(INode sourceNode)
        {

        }
    }
}