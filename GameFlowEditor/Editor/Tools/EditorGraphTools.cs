namespace UniModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.GameFlowEditor.Editor.Tools
{
    using System.Linq;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Core;
    using UnityEngine;

    public static class EditorGraphTools
    {
        public static UniGraph FindSceneGraph(string graphName)
        {
            var target = NodeGraph.ActiveGraphs.
                OfType<UniGraph>().
                FirstOrDefault(x => x.name == graphName);
            if (target) return target;
            target = Object.FindObjectsOfType<UniGraph>().
                FirstOrDefault(x => x.name == graphName);
            return target;
        }
    }
}
