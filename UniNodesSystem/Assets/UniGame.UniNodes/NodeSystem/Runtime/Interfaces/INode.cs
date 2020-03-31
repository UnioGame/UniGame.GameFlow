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
            IReadOnlyList<Type> types, PortIO direction,
            ConnectionType connectionType = ConnectionType.Multiple,
            ShowBackingValue showBackingValue = ShowBackingValue.Always);
        
        
        void Validate();

        bool AddPortValue(INodePort portValue);
        
        #region editor api

        int SetId(int id);
        
        /// <summary>
        /// set up graph node position
        /// </summary>
        new Vector2 Position { get; set; }

        /// <summary>
        /// setup node view width
        /// </summary>
        new int Width { get; set; }
        
        #endregion

    }
}