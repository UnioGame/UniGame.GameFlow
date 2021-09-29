namespace UniModules.GameFlow.Runtime.Core
{
    using Interfaces;
    using Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;

    public interface IGraphData : INamedItem, IUnique
    {
        int GetNextId();
        
        int UpdateId(int oldId);
        
        INode GetNode(int nodeId);

        /// <summary> Safely remove a node and all its connections </summary>
        /// <param name="node"> The node to remove </param>
        IGraphData RemoveNode(INode node);

        IContext GraphContext { get; }
    }
}