using UniModules.GameFlow.Runtime.Core;
using UniModules.UniGame.Context.Runtime.Connections;

namespace UniModules.GameFlow.Runtime.Interfaces
{
    using System.Collections.Generic;
    using Core.Interfaces;
    using global::UniGame.Core.Runtime;
    using IGraphData = Core.IGraphData;
    using Vector2 = UnityEngine.Vector2;

    public interface IImmutableNode : IGraphItem, IEditorNode
    {
        NodeGraph GraphData { get; }

        IContextConnection Context { get; }

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