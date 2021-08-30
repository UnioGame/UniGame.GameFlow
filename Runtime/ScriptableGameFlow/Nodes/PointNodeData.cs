namespace UniModules.UniGame.GameFlow.GameFlowEditor.Runtime.Nodes
{
    using Editor.Attributes;
    using GameFlow.Runtime.Nodes.Common;
    using global::UniGame.GameFlowEditor.Runtime;
    using GraphProcessor;
    using UnityEngine;
    using UnityEngine.UIElements;

    [NodeBind(typeof(PointNodeData),typeof(PointNode))]
    [NodeMenuItem("Point")]
    [System.Serializable]
    public class PointNodeData : UniBaseNode
    {
        private static string nodeStyle = "GameFlow/UCSS/PortsOnlyStyle";
        
        public override string layoutStyle => nodeStyle;

        //[IsCompatibleWithGraph]
        public static bool Create(BaseGraph assetGraph)
        {
            var uniGraph = assetGraph as UniGraphAsset;
            
            if (!uniGraph)
                return false;

            uniGraph.CreateNode(typeof(PointNode), Vector2.zero);
            return true;
        }
    }
}