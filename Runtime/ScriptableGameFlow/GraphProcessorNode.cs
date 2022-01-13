using GraphProcessor;
using UniModules.GameFlow.Runtime.Core;
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
        private readonly UniGraph graph;
        private readonly BaseNode node;

        public override string ItemName => node.name;

        public GraphProcessorNode(UniGraph graph,BaseNode node)
        {
            this.graph = graph;
            this.node = node;
        }


        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            
        }
    }

    public interface IRuntimeOnlyNode
    {
        
    }
}
