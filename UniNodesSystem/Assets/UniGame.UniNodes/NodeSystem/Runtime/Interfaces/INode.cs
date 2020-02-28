namespace UniGame.UniNodes.NodeSystem.Runtime.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Core;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UnityEngine;

    public interface INode : IImmutableNode
    {

        void SetUpData(IGraphData data);

        void SetName(string nodeName);
        
        /// <summary> Remove an instance port from the node </summary>
        void RemovePort(string fieldName);

        /// <summary> Remove an instance port from the node </summary>
        void RemovePort(NodePort port);

        /// <summary> Disconnect everything from this node </summary>
        void ClearConnections();
        
        void Initialize(IGraphData data);
        
        NodePort AddPort(
            string fieldName,
            IReadOnlyList<Type> types, PortIO direction,
            ConnectionType connectionType = ConnectionType.Multiple,
            ShowBackingValue showBackingValue = ShowBackingValue.Always);
        
        #region editor api
        
        /// <summary>
        /// set up graph node position
        /// </summary>
        void SetPosition(Vector2 position);

        /// <summary>
        /// setup node view width
        /// </summary>
        void SetWidth(int nodeWidth);

        #endregion

    }
}