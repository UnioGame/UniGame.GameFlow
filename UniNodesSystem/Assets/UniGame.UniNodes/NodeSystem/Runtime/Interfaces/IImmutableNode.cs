namespace UniGame.UniNodes.NodeSystem.Runtime.Interfaces
{
    using System.Collections.Generic;
    using Core;
    using Core.Interfaces;
    using IGraphData = Core.IGraphData;
    using Vector2 = UnityEngine.Vector2;

    public interface IImmutableNode : IGraphItem
    {

        IGraphData GraphData { get; }

        IReadOnlyList<NodePort> Ports { get; }

        /// <summary> Iterate over all outputs on this node. </summary>
        IEnumerable<NodePort> Outputs { get; }

        /// <summary> Iterate over all inputs on this node. </summary>
        IEnumerable<NodePort> Inputs { get; }

        /// <summary> Returns output port which matches fieldName </summary>
        NodePort GetOutputPort(string fieldName);

        /// <summary> Returns input port which matches fieldName </summary>
        NodePort GetInputPort(string fieldName);

        /// <summary> Returns port which matches fieldName </summary>
        NodePort GetPort(string fieldName);

        bool HasPort(string fieldName);

        #region Editor Api
        
        
        Vector2 Position { get; }

        int Width { get; }

        #endregion
        
    }
}