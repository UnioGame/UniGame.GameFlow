namespace UniGame.UniNodes.NodeSystem.Runtime.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Core;
    using UnityEngine;

    public interface INode : IImmutableNode
    {

        void SetUpData(IGraphData data);

        void SetName(string nodeName);
        
        /// <summary> Remove an instance port from the node </summary>
        void RemovePort(string fieldName);

        /// <summary> Remove an instance port from the node </summary>
        void RemovePort(INodePort port);

        /// <summary> Disconnect everything from this node </summary>
        void ClearConnections();
        
        void Initialize(IGraphData data);

        NodePort AddPort(
            string fieldName,
            IEnumerable<Type> types, 
            PortIO direction,
            ConnectionType connectionType = ConnectionType.Multiple,
            ShowBackingValue showBackingValue = ShowBackingValue.Always);
        
        //NodePort AddPort(INodePort port);
        
        void Validate();

    }
}