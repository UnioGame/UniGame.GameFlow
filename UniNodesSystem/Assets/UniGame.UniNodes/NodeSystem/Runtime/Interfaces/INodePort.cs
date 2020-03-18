namespace UniGame.UniNodes.NodeSystem.Runtime.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Core;
    using Core.Interfaces;
    using UnityEngine;

    public interface INodePort : IGraphItem
    {

        int ConnectionCount { get; }

        /// <summary> Return the first non-null connection </summary>
        INodePort Connection { get; }

        IPortValue Value { get; }

        IReadOnlyList<IPortConnection> Connections { get; }

        PortIO Direction { get; }
        
        ConnectionType ConnectionType { get; }

        /// <summary> Is this port connected to anytihng? </summary>
        bool IsConnected { get; }

        INode Node { get; }

        IReadOnlyList<Type> ValueTypes { get; }
        
        int UpdateId();

        /// <summary> Checks all connections for invalid references, and removes them. </summary>
        void VerifyConnections();

        IPortConnection CreateConnection(int portid, int nodeId,string portName);
        
        void RemoveConnection(IPortConnection connection);
        
        /// <summary> Connect this <see cref="NodePort"/> to another </summary>
        /// <param name="port">The <see cref="NodePort"/> to connect to</param>
        void Connect(INodePort port);

        IEnumerable<INodePort> GetConnections();
        
        INodePort GetConnection(int i);

        /// <summary> Get index of the connection connecting this and specified ports </summary>
        int GetConnectionIndex(INodePort port);

        bool IsConnectedTo(INodePort port);

        /// <summary> Disconnect this port from another port </summary>
        void Disconnect(INodePort port);

        /// <summary> Disconnect this port from another port </summary>
        void Disconnect(int i);

        void ClearConnections();

        /// <summary> Get reroute points for a given connection. This is used for organization </summary>
        List<Vector2> GetReroutePoints(int index);
        
        /// <summary> Copy all connections pointing to a node and add them to this one </summary>
        void AddConnections(INodePort targetPort);

    }
}