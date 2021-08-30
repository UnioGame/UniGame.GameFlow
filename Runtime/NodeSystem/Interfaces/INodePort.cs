namespace UniModules.GameFlow.Runtime.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Core;
    using Core.Interfaces;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniRx;
    using UnityEngine;

    public interface INodePort : 
        INamedItem, 
        IBroadcaster<IMessagePublisher>,
        IPoolable
    {
        Type ValueType { get; }

        bool IsInput { get; }

        bool IsOutput { get; }

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

        int NodeId { get; }

        int PortId { get; }

        IReadOnlyList<Type> ValueTypes { get; }

        void Initialize(INode node);
        
        /// <summary> Checks all connections for invalid references, and removes them. </summary>
        void VerifyConnections();

        IPortConnection CreateConnection(int nodeId,string portName);
        
        void RemoveConnection(IPortConnection connection);
        
        /// <summary> Connect this <see cref="NodePort"/> to another </summary>
        /// <param name="port">The <see cref="NodePort"/> to connect to</param>
        void Connect(INodePort port);

        IEnumerable<INodePort> GetConnections();
        
        INodePort GetConnection(int i);

        void SetPortData(IPortData portData);
        
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

        void SwapConnections(INodePort targetPort);

    }
}