namespace UniGame.UniNodes.NodeSystem.Runtime.Interfaces
{
    using System.Collections.Generic;
    using Core.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using IGraphData = Core.IGraphData;
    using Vector2 = UnityEngine.Vector2;

    public interface IImmutableNode : IGraphItem, IEditorNode
    {
        IGraphData GraphData { get; }

        IEnumerable<INodePort> Ports { get; }

        /// <summary> Iterate over all outputs on this node. </summary>
        IEnumerable<INodePort> Outputs { get; }

        /// <summary> Iterate over all inputs on this node. </summary>
        IEnumerable<INodePort> Inputs { get; }

        /// <summary> Returns output port which matches fieldName </summary>
        INodePort GetOutputPort(string fieldName);

        /// <summary> Returns input port which matches fieldName </summary>
        INodePort GetInputPort(string fieldName);

        /// <summary> Returns port which matches fieldName </summary>
        INodePort GetPort(string fieldName);

        bool HasPort(string fieldName);

    }
}