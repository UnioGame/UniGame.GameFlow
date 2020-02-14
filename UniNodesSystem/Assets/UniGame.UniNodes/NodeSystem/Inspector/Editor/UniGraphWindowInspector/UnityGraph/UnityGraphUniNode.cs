﻿using Node = UnityEditor.Graphs.Node;

namespace UniGreenModules.UniNodeSystem.Inspector.Editor.UnityGraph
{
    using Runtime.Core;

    public class UnityGraphUniNode : UnityEditor.Graphs.Node
    {
    
        #region factory methods

        public static UnityEditor.Graphs.Node Create(Node graphNode)
        {
            var node = CreateInstance<UnityGraphUniNode>();
            node.Initialize(graphNode);
            return node;
        }
        
        #endregion

        private Node _node;
        
        public void Initialize(Node graphNode)
        {
            _node = graphNode;
        }
        
    }
}
