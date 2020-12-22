namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using Interfaces;
    using Runtime.Interfaces;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.Attributes;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniRx;
    using UnityEngine;

    [Serializable]
    public class NodePort : INodePort, IPortData, IEqualityComparer<INodePort>
    {
        #region inspector

        /// <summary>
        /// unique graph space port id 
        /// </summary>
        [ReadOnlyValue]
        [SerializeField]
        private int id;
        /// <summary>
        /// parent Node id
        /// </summary>
        [ReadOnlyValue]
        [SerializeField]
        public int nodeId;
        [SerializeField] public string           fieldName;
        [SerializeField] public PortIO           direction          = PortIO.Input;
        [SerializeField] public ConnectionType   connectionType     = ConnectionType.Multiple;
        [SerializeField] public ShowBackingValue showBackingValue   = ShowBackingValue.Always;
        [SerializeField] public bool             isDynamic          = true;
        [SerializeField] public bool             isInstancePortList = false;
        /// <summary>
        /// port container value
        /// </summary>
        [SerializeField]
        public PortValue portValue = new PortValue();
        /// <summary>
        /// registered port connections
        /// </summary>
        [SerializeField]
        public List<PortConnection> connections = new List<PortConnection>();
        /// <summary>
        /// dynamic port list
        /// </summary>
        [SerializeField] public bool instancePortList;

        #endregion

        [NonSerialized] public LifeTimeDefinition lifeTimeDefinition;
        /// <summary>
        /// port lifeTime 
        /// </summary>
        [NonSerialized]
        private ILifeTime lifeTime;

        /// <summary>
        /// port parent info
        /// </summary>    
        [NonSerialized]
        public INode node;

        [NonSerialized]
        private IPortConnectionValidator connectionValidator;

        #region constructor

        /// <summary>
        /// Copy a nodePort but assign it to another node.
        /// </summary>
        public NodePort(int id,NodePort nodePort, INode node) :
            this(id,
                node,
                nodePort.ItemName,
                nodePort.Direction,
                nodePort.ConnectionType,
                nodePort.ShowBackingValue,
                nodePort.Value.ValueTypes)
        {
            connections.Clear();
            connections.AddRange(nodePort.connections);
        }

        public NodePort(int id,INode node, IPortData portData) :
            this(id,
                node,
                portData.ItemName,
                portData.Direction,
                portData.ConnectionType,
                portData.ShowBackingValue,
                portData.ValueTypes)
        {
        }

        /// <summary>
        /// Construct a dynamic port. Dynamic ports are not forgotten on reimport,
        /// and is ideal for runtime-created ports.
        /// </summary>
        public NodePort(
            int id,
            INode node,
            string fieldName,
            Type type,
            PortIO direction = PortIO.Input,
            ConnectionType connectionType = ConnectionType.Multiple,
            ShowBackingValue showBackingValue = ShowBackingValue.Always) :
            this(id,node, fieldName, direction, connectionType, showBackingValue, new List<Type>() {type})
        {
        }


        public NodePort(
            int id,
            INode node,
            string fieldName,
            PortIO direction = PortIO.Input,
            ConnectionType connectionType = ConnectionType.Multiple,
            ShowBackingValue showBackingValue = ShowBackingValue.Always,
            IEnumerable<Type> types = null)
        {
            this.id = id;
            this.node   = node;
            this.nodeId = node.Id;
            this.fieldName        = fieldName;
            this.direction        = direction;
            this.connectionType   = connectionType;
            this.showBackingValue = showBackingValue;
            portValue.SetValueTypeFilter(types);
            
            Initialize(node);
        }

        #endregion

        #region public properties

        public int Id => id;

        public IReadOnlyList<IPortConnection> Connections => connections;


        public IPortConnectionValidator ConnectionValidator => connectionValidator = 
            connectionValidator ?? 
            new PortConnectionValidator();

        public bool InstancePortList => instancePortList;

        public int PortId => Id;
        
        public IReadOnlyList<Type> ValueTypes => portValue.ValueTypes;

        public int ConnectionCount => connections.Count;

        /// <summary> Return the first non-null connection </summary>
        public INodePort Connection {
            get {
                for (var i = 0; i < connections.Count; i++) {
                    if (connections[i] != null) return connections[i].Port;
                }

                return null;
            }
        }

        public IPortValue Value => portValue;

        public PortIO         Direction      => direction;
        public ConnectionType ConnectionType => connectionType;

        /// <summary> Is this port connected to anytihng? </summary>
        public bool IsConnected => connections.Count != 0;

        public bool   IsInput  => Direction == PortIO.Input;
        public bool   IsOutput => Direction == PortIO.Output;
        public string ItemName => fieldName;

        public ShowBackingValue ShowBackingValue => showBackingValue;
        public Type             ValueType        => portValue.ValueTypes.FirstOrDefault();

        public INode Node => node;

        public ILifeTime LifeTime => lifeTime;

        public int NodeId => nodeId;

        #endregion

        public void Initialize(INode data)
        {
            this.node   = data;
            this.nodeId = data.Id;

            connections = connections ?? new List<PortConnection>();
            connections.ForEach(x => x.Initialize(node.GraphData));

            //initialize port lifetime
            lifeTimeDefinition = lifeTimeDefinition ?? new LifeTimeDefinition();
            lifeTime           = lifeTimeDefinition.LifeTime;
            lifeTimeDefinition.Release();

            //initialize port value
            portValue.Initialize(fieldName);
            //bind port value to port lifetime
            lifeTime.AddDispose(portValue);
        }

        #region port value methods

        public void SetPortData(IPortData portData)
        {
            fieldName        = portData.ItemName;
            direction        = portData.Direction;
            connectionType   = portData.ConnectionType;
            showBackingValue = portData.ShowBackingValue;
            portValue.SetValueTypeFilter(portData.ValueTypes);
        }

        #endregion

        /// <summary>
        /// terminate Port lifetime, release resources
        /// </summary>
        public void Release() =>  lifeTimeDefinition.Terminate();

        /// <summary>
        /// connect current port to publishers
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public IDisposable Broadcast(IMessagePublisher connection) => Value.Broadcast(connection);

        #region comperer api

        public bool Equals(INodePort x, INodePort y)
        {
            return x?.NodeId == y?.NodeId && x?.ItemName == y?.ItemName;
        }

        public int GetHashCode(INodePort obj)
        {
            var hash = 89 * obj.NodeId + obj.ItemName.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is INodePort port))
                return false;

            return port.NodeId == NodeId && ItemName == port.ItemName;
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }
        

        #endregion
        
        #region node methods

        /// <summary> Checks all connections for invalid references, and removes them. </summary>
        public void VerifyConnections()
        {
            var removedConnections = ClassPool.Spawn<List<PortConnection>>();

            for (var i = connections.Count - 1; i >= 0; i--) {
                var connection = connections[i];
                var targetPort = connection.Port;
                if (targetPort != null)
                    continue;
                removedConnections.Add(connection);
            }

            removedConnections.ForEach(x => connections.Remove(x));
            removedConnections.Despawn();
        }

        /// <summary> Connect this <see cref="NodePort"/> to another </summary>
        /// <param name="port">The <see cref="NodePort"/> to connect to</param>
        public void Connect(INodePort port)
        {
            if (connections == null)
                connections = new List<PortConnection>();

            if (!ConnectionValidator.Validate(this, port)) {
                GameLog.LogError($"{node?.GraphData.ItemName}:{Node?.ItemName}:{ItemName} Connection Error. Validation Failed");
                return;
            }

            if (port.ConnectionType == ConnectionType.Override && port.ConnectionCount != 0) {
                port.ClearConnections();
            }

            if (ConnectionType == ConnectionType.Override && ConnectionCount != 0) {
                ClearConnections();
            }

            var portNode = port.Node;
            connections.Add(new PortConnection(port, portNode.Id));

            if (!port.IsConnectedTo(this))
                port.CreateConnection(nodeId, ItemName);
        }

        public IPortConnection CreateConnection(int targetNodeId, string portName)
        {
            var portConnection = new PortConnection(targetNodeId, portName);
            portConnection.Initialize(node.GraphData);
            connections.Add(portConnection);
            return portConnection;
        }

        public IEnumerable<INodePort> GetConnections()
        {
            for (var i = 0; i < connections.Count; i++) {
                var port = GetConnection(i);
                if (port != null)
                    yield return port;
            }
        }

        public INodePort GetConnection(int i)
        {
            if (i >= connections.Count || i < 0) return null;

            var connection = connections[i];
            //If the connection is broken for some reason, remove it.
            return connection.Port;
        }

        public void Validate() => VerifyConnections();

        /// <summary> Get index of the connection connecting this and specified ports </summary>
        public int GetConnectionIndex(INodePort port)
        {
            for (var i = 0; i < ConnectionCount; i++) {
                var connection = connections[i];
                if(connection.NodeId == port.NodeId && connection.PortName == port.ItemName)
                    return i;
            }

            return -1;
        }

        public bool IsConnectedTo(INodePort port)
        {
            for (var i = 0; i < connections.Count; i++) {
                var connection = connections[i];
                if (connection.IsTarget(port)) {
                    return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Disconnect this port from another port
        /// </summary>
        public void Disconnect(INodePort port)
        {
            // Remove this ports connection to the other
            if (port == null) {
                return;
            }

            for (var i = connections.Count - 1; i >= 0; i--) {
                var connection = connections[i];
                if (connection.IsTarget(port)) {
                    connections.RemoveAt(i);
                }
            }

            // Remove the other ports connection to this port
            for (var i = 0; i < port.Connections.Count; i++) {
                var connection = port.Connections[i];
                if (connection.IsTarget(this)) {
                    port.RemoveConnection(connection);
                }
            }
        }

        public void RemoveConnection(IPortConnection connection)
        {
            if (connection is PortConnection portConnection) {
                var index = connections.IndexOf(portConnection);
                connections.RemoveAt(index);
            }
        }

        /// <summary> Disconnect this port from another port </summary>
        public void Disconnect(int i)
        {
            // Remove the other ports connection to this port
            var otherPort = connections[i].GetPort();
            if (otherPort != null) {
                for (var k = 0; k < otherPort.Connections.Count; k++) {
                    var connection = otherPort.Connections[i];
                    if (connection.IsTarget(this)) {
                        otherPort.RemoveConnection(connection);
                    }
                }
            }

            // Remove this ports connection to the other
            connections.RemoveAt(i);
        }

        public void ClearConnections()
        {
            var removedConnections = ClassPool.Spawn<List<PortConnection>>();
            removedConnections.AddRange(connections);

            foreach (var connection in removedConnections) {
                if (connection.Port == null) continue;
                Disconnect(connection.Port);
            }

            connections.Clear();
            removedConnections.Despawn();
        }

        /// <summary> Get reroute points for a given connection. This is used for organization </summary>
        public List<Vector2> GetReroutePoints(int index)
        {
            return connections[index].reroutePoints;
        }

        /// <summary> Copy all connections pointing to a node and add them to this one </summary>
        public void AddConnections(INodePort targetPort)
        {
            var connectionCount = targetPort.ConnectionCount;
            for (var i = 0; i < connectionCount; i++) {
                var connection = targetPort.Connections[i];
                var otherPort  = connection.Port;
                Connect(otherPort);
            }
        }

        /// <summary> Swap connections with another node </summary>
        public void SwapConnections(INodePort targetPort)
        {
            var aConnectionCount = connections.Count;
            var bConnectionCount = targetPort.Connections.Count;

            var portConnections       = new List<INodePort>();
            var targetPortConnections = new List<INodePort>();

            // Cache port connections
            for (var i = 0; i < aConnectionCount; i++)
                portConnections.Add(connections[i].Port);

            // Cache target port connections
            for (var i = 0; i < bConnectionCount; i++)
                targetPortConnections.Add(targetPort.Connections[i].Port);

            ClearConnections();

            targetPort.ClearConnections();

            // Add port connections to targetPort
            for (var i = 0; i < portConnections.Count; i++)
                targetPort.Connect(portConnections[i]);

            // Add target port connections to this one
            for (var i = 0; i < targetPortConnections.Count; i++)
                Connect(targetPortConnections[i]);
        }
        
        #endregion

    }
}