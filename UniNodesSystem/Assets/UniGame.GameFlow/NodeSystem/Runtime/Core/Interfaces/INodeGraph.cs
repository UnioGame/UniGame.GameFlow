namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using Runtime.Interfaces;
    using Vector2 = UnityEngine.Vector2;

    public interface INodeGraph : IDisposable, IGraphData
    {
        List<INode> Nodes { get; }

        IGraphData GraphData { get; }

        /// <summary> Add a node to the graph by type </summary>
        T AddNode<T>() 
            where T : class, INode;

        T AddNode<T>(string name) 
            where T : class, INode;

        INode AddNode(Type type, string itemName, Vector2 nodePosition);
        
        INode AddNode(string itemName, Type type);

        /// <summary> Add a node to the graph by type </summary>
        INode AddNode(Type type);

        /// <summary> Safely remove a node and all its connections </summary>
        /// <param name="node"> The node to remove </param>
        IGraphData RemoveNode(INode node);
    }
}