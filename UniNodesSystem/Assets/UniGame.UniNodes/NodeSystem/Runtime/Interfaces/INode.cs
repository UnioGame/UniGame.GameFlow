namespace UniGreenModules.UniNodeSystem.Runtime.Interfaces
{
    using System.Collections.Generic;
    using Core;
    using UniCore.Runtime.Interfaces;

    public interface INode : IMutableNode , INamedItem
    {
        ulong Id { get; }

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

        string GetName();
    }
}