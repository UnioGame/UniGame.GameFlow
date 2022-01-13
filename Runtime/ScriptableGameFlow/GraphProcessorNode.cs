using GraphProcessor;
using UniModules.GameFlow.Runtime.Interfaces;

namespace UniGame.GameFlow
{
    using System;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniModules.GameFlow.Runtime.Core.Nodes;

    [Serializable]
    [HideNode]
    public class GraphProcessorNode : SNode,IRuntimeOnlyNode
    {
        private readonly BaseNode node;

        public override string ItemName => node.name;

        public GraphProcessorNode(BaseNode node)
        {
            this.node = node;
        }
    }

    public interface IRuntimeOnlyNode
    {
        
    }
}
