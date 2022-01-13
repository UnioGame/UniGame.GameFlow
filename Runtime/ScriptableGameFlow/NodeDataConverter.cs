using System.Collections.Generic;
using UniGame.GameFlowEditor.Runtime;
using UniModules.GameFlow.Runtime.Core;
using UniModules.GameFlow.Runtime.Interfaces;

namespace UniGame.GameFlow
{
    public class NodeDataConverter
    {
        public static IEnumerable<INode> ConvertNodes(UniGraph graph,UniGraphAsset graphAsset)
        {
            var nodes = graphAsset.nodes;
            foreach (var node in nodes)
            {
                if(node is UniBaseNode) continue;

                var bufferNode = new GraphProcessorNode(graph,node);
                yield return bufferNode;
            }
        }
    }
}
