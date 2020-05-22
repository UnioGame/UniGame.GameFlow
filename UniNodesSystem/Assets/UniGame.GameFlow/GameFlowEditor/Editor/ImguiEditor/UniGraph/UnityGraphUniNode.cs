namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.UnityGraph
{
    using Runtime.Core;
    using Runtime.Interfaces;

    public class UnityGraphUniNode : UnityEditor.Graphs.Node
    {
    
        private INode _node;

        #region factory methods

        public static UnityEditor.Graphs.Node Create(INode graphNode)
        {
            var node = CreateInstance<UnityGraphUniNode>();
            node.Initialize(graphNode);
            return node;
        }
        
        #endregion

        public void Initialize(INode graphNode)
        {
            _node = graphNode;
        }
        
    }
}
