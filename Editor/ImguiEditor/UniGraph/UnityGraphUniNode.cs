namespace UniGame.GameFlow.Editor.UnityGraph
{
    using UniModules.GameFlow.Runtime.Interfaces;

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
