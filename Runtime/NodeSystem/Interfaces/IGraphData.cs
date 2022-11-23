using UniModules.UniGame.Context.Runtime.Connections;

namespace UniModules.GameFlow.Runtime.Core
{
    using Interfaces;
    using Runtime.Interfaces;
    using global::UniGame.Core.Runtime;
    using UnityEngine;

    public interface IGraphData : INamedItem, IUnique
    {
        Transform Root { get; }

        int GetNextId();
        
        int UpdateId(int oldId);
        
        INode GetNode(int nodeId);

        /// <summary> Safely remove a node and all its connections </summary>
        /// <param name="node"> The node to remove </param>
        IGraphData RemoveNode(INode node);

        IContextConnection GraphContext { get; }
    }
}