using System.Collections.Generic;
using UniGame.GameFlowEditor.Runtime;
using UniModules.GameFlow.Runtime.Interfaces;

namespace UniGame.GameFlow
{
    using UnityEngine;
    
    public class NodeDataConverter
    {
        public static IEnumerable<INode> ConvertNodes(UniGraphAsset graphAsset)
        {
            var nodes = graphAsset.nodes;
            foreach (var node in nodes)
            {
                if(node is UniBaseNode) continue;

                var bufferNode = new GraphProcessorNode(node);
                yield return bufferNode;
            }
        }
    }
}
