namespace UniGreenModules.UniNodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Interfaces;
    using UniCore.Runtime.Attributes;
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniCore.Runtime.ObjectPool.Runtime;
    using UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniCore.Runtime.ProfilerTools;
    using UnityEngine;

    [Serializable]
    public class NodePort : INodePort
    {
        #region inspector
        
        [ReadOnlyValue] 
        [SerializeField] 
        private ulong _id;

        [SerializeField]
        public UniPortValue _portValue = new UniPortValue();

        [SerializeField] private string               _fieldName;
        [SerializeField] private UniBaseNode          _node;
        [SerializeField] private string               _typeQualifiedName;
        [SerializeField] private List<PortConnection> connections = new List<PortConnection>();
        [SerializeField] private PortIO               _direction;
        [SerializeField] private ConnectionType       _connectionType;
        [SerializeField] private bool                 _dynamic;
        
        /// <summary>
        /// allowed port value types
        /// </summary>
        [SerializeField] private List<string> _valueTypeNames;

        #endregion
        
        #region  constructor

        /// <summary> Construct a static targetless nodeport. Used as a template. </summary>
        public NodePort(FieldInfo fieldInfo)
        {
            _fieldName = fieldInfo.Name;
            _dynamic   = false;
            
            _typeQualifiedName = fieldInfo.FieldType.AssemblyQualifiedName;
            
            var attribs = fieldInfo.GetCustomAttributes(false);
            
            for (var i = 0; i < attribs.Length; i++) {
                switch (attribs[i]) {
                    case NodeInputAttribute atr:
                        _direction      = PortIO.Input;
                        _connectionType = atr.connectionType;
                        break;
                    case NodeOutputAttribute atr:
                        _direction      = PortIO.Output;
                        _connectionType = atr.connectionType;
                        break;
                }
            }
            
            _portValue = new UniPortValue(new List<string>(){_typeQualifiedName});
        }

        /// <summary> Copy a nodePort but assign it to another node. </summary>
        public NodePort(NodePort nodePort, UniBaseNode node) :
            this(nodePort._fieldName,nodePort.ValueType,nodePort._direction,nodePort.connectionType,node)
        {
        }

        /// <summary> Construct a dynamic port. Dynamic ports are not forgotten on reimport, and is ideal for runtime-created ports. </summary>
        public NodePort(string fieldName, Type type, PortIO direction, ConnectionType connectionType, UniBaseNode node) :
            this(fieldName,new List<Type>(){type},direction,connectionType,node )
        {
        }
        
        
        public NodePort(string fieldName, List<Type> types, PortIO direction, ConnectionType connectionType, UniBaseNode node)
        {
            var typeFilterNames = types.Select(x => x.AssemblyQualifiedName).ToList();
            _typeQualifiedName = typeFilterNames.FirstOrDefault();
            
            _fieldName         = fieldName;
            _direction         = direction;
            _node              = node;
            _dynamic           = true;
            _connectionType    = connectionType;
            _portValue         = new UniPortValue(typeFilterNames);
        }

        #endregion
        
        #region public properties

        //public IPortValue Value => _portValue;
        
        public ulong Id => _id = _id == 0 ? UpdateId() : _id;

        
        private List<Type> portValueTypes = new List<Type>();

        public List<Type> ValueTypes => portValueTypes ?? (_valueTypeNames == null ? 
                                            new List<Type>() : 
                                            _valueTypeNames.Select(x => Type.GetType(x, false)).ToList()); 
        
        public int ConnectionCount => connections.Count;

        /// <summary> Return the first non-null connection </summary>
        public NodePort Connection {
            get {
                for (var i = 0; i < connections.Count; i++) {
                    if (connections[i] != null) return connections[i].Port;
                }
                return null;
            }
        }

        public PortIO         direction      => _direction;
        public ConnectionType connectionType => _connectionType;

        /// <summary> Is this port connected to anytihng? </summary>
        public bool IsConnected => connections.Count != 0;

        public bool IsInput  => direction == PortIO.Input;
        public bool IsOutput => direction == PortIO.Output;

        public string fieldName => _fieldName;

        public bool IsDynamic => _dynamic;

        public bool IsStatic => !_dynamic;

        public UniBaseNode node {
            get => _node;
            set {
                if (_node == value)
                    return;
                _node = value;
                _id   = _node.graph.GetId();
            }
        }

        private Type valueType;
        public Type ValueType => valueType = valueType == null && !string.IsNullOrEmpty(_typeQualifiedName) ? 
            Type.GetType(_typeQualifiedName, false) : valueType;

        #endregion

        #region value methods

        public T GetValue<T>() => _portValue.Get<T>();

        public bool Contains<T>() => _portValue.Contains<T>();
        
        public List<T> GetValues<T>()
        {
            var values = ClassPool.Spawn<List<T>>();

            if (direction == PortIO.Output)
                return values;

            for (var i = 0; i < connections.Count; i++) {
                var connection = connections[i];
                var port = connection.Port;
                var hasValue = port.Contains<T>();
                if(hasValue) values.Add(port.GetValue<T>());
            }
            
            return values;
        }
        
        public void Publish<T>(T message) => _portValue.Publish<T>(message);

        public IObservable<T> Receive<T>()
        {
            return _portValue.Receive<T>();
        }
        
        #endregion
        
        #region node methods

        public void Initialize(ILifeTime lifeTime)
        {
            _portValue.Initialize(_fieldName,lifeTime);
        }

        public void SetValueFilter(List<Type> filter)
        {
            
        }
        
        public ulong UpdateId() => _id = node.graph.GetId();

        /// <summary> Checks all connections for invalid references, and removes them. </summary>
        public void VerifyConnections()
        {
            for (var i = connections.Count - 1; i >= 0; i--) {
                if (connections[i].node != null &&
                    !string.IsNullOrEmpty(connections[i].fieldName) &&
                    connections[i].node.GetPort(connections[i].fieldName) != null)
                    continue;
                connections.RemoveAt(i);
            }
        }

        /// <summary> Connect this <see cref="NodePort"/> to another </summary>
        /// <param name="port">The <see cref="NodePort"/> to connect to</param>
        public void Connect(NodePort port)
        {
            if (connections == null) connections = new List<PortConnection>();
            if (port == null) {
                GameLog.LogWarning("Cannot connect to null port");
                return;
            }

            if (port == this) {
                GameLog.LogWarning("Cannot connect port to self.");
                return;
            }

            if (IsConnectedTo(port)) {
                GameLog.LogWarning("Port already connected. ");
                return;
            }

            if (direction == port.direction) {
                GameLog.LogWarning("Cannot connect two " + (direction == PortIO.Input ? "input" : "output") + " connections");
                return;
            }

            if (port.connectionType == ConnectionType.Override && port.ConnectionCount != 0) {
                port.ClearConnections();
            }

            if (connectionType == ConnectionType.Override && ConnectionCount != 0) {
                ClearConnections();
            }

            connections.Add(new PortConnection(port));
            if (port.connections == null) port.connections = new List<PortConnection>();
            if (!port.IsConnectedTo(this)) port.connections.Add(new PortConnection(this));
            node.OnCreateConnection(this, port);
            port.node.OnCreateConnection(this, port);
        }

        public List<NodePort> GetConnections()
        {
            var result = ClassPool.Spawn<List<NodePort>>();
            for (var i = 0; i < connections.Count; i++) {
                var port = GetConnection(i);
                if (port != null) result.Add(port);
            }

            return result;
        }

        public NodePort GetConnection(int i)
        {
            var connection = connections[i];
            //If the connection is broken for some reason, remove it.
            if (connection.node == null || string.IsNullOrEmpty(connection.fieldName)) {
                return null;
            }

            var port = connection.node.GetPort(connection.fieldName);
            return port ?? null;
        }

        public void Validate()
        {
            var tempConnections = ClassPool.Spawn<List<PortConnection>>();
            tempConnections.AddRange(connections);

            for (var i = 0; i < tempConnections.Count; i++) {
                var connection = tempConnections[i];
                if (connection.node == null ||
                    string.IsNullOrEmpty(connection.fieldName)) {
                    connections.Remove(connection);
                    continue;
                }

                var port = connection.node.GetPort(connection.fieldName);
                if (port == null) {
                    connections.Remove(connection);
                }
            }

            tempConnections.DespawnCollection();
        }

        /// <summary> Get index of the connection connecting this and specified ports </summary>
        public int GetConnectionIndex(NodePort port)
        {
            for (var i = 0; i < ConnectionCount; i++) {
                if (connections[i].Port == port) return i;
            }

            return -1;
        }

        public bool IsConnectedTo(NodePort port)
        {
            for (var i = 0; i < connections.Count; i++) {
                if (connections[i].Port == port) return true;
            }

            return false;
        }

        /// <summary> Disconnect this port from another port </summary>
        public void Disconnect(NodePort port)
        {
            // Remove this ports connection to the other
            for (var i = connections.Count - 1; i >= 0; i--) {
                var connection = connections[i];
                if (connection.Port == port) {
                    connections.RemoveAt(i);
                }
            }

            if (port != null) {
                // Remove the other ports connection to this port
                for (var i = 0; i < port.connections.Count; i++) {
                    if (port.connections[i].Port == this) {
                        port.connections.RemoveAt(i);
                    }
                }
            }

            // Trigger OnRemoveConnection
            node.OnRemoveConnection(this);
            port?.node.OnRemoveConnection(port);
        }

        /// <summary> Disconnect this port from another port </summary>
        public void Disconnect(int i)
        {
            // Remove the other ports connection to this port
            var otherPort = connections[i].Port;
            if (otherPort != null) {
                for (var k = 0; k < otherPort.connections.Count; k++) {
                    if (otherPort.connections[k].Port == this) {
                        otherPort.connections.RemoveAt(i);
                    }
                }
            }

            // Remove this ports connection to the other
            connections.RemoveAt(i);

            // Trigger OnRemoveConnection
            node.OnRemoveConnection(this);
            otherPort?.node.OnRemoveConnection(otherPort);
        }

        public void ClearConnections()
        {
            while (connections.Count > 0) {
                Disconnect(connections[0].Port);
            }
        }

        /// <summary> Get reroute points for a given connection. This is used for organization </summary>
        public List<Vector2> GetReroutePoints(int index)
        {
            return connections[index].reroutePoints;
        }

        /// <summary> Swap connections with another node </summary>
        public void SwapConnections(NodePort targetPort)
        {
            var aConnectionCount = connections.Count;
            var bConnectionCount = targetPort.connections.Count;

            var portConnections       = new List<NodePort>();
            var targetPortConnections = new List<NodePort>();

            // Cache port connections
            for (var i = 0; i < aConnectionCount; i++)
                portConnections.Add(connections[i].Port);

            // Cache target port connections
            for (var i = 0; i < bConnectionCount; i++)
                targetPortConnections.Add(targetPort.connections[i].Port);

            ClearConnections();
            targetPort.ClearConnections();

            // Add port connections to targetPort
            for (var i = 0; i < portConnections.Count; i++)
                targetPort.Connect(portConnections[i]);

            // Add target port connections to this one
            for (var i = 0; i < targetPortConnections.Count; i++)
                Connect(targetPortConnections[i]);
        }

        /// <summary> Copy all connections pointing to a node and add them to this one </summary>
        public void AddConnections(NodePort targetPort)
        {
            var connectionCount = targetPort.ConnectionCount;
            for (var i = 0; i < connectionCount; i++) {
                var connection = targetPort.connections[i];
                var otherPort  = connection.Port;
                Connect(otherPort);
            }
        }

        /// <summary> Move all connections pointing to this node, to another node </summary>
        public void MoveConnections(NodePort targetPort)
        {
            var connectionCount = connections.Count;

            // Add connections to target port
            for (var i = 0; i < connectionCount; i++) {
                var connection = targetPort.connections[i];
                var otherPort  = connection.Port;
                Connect(otherPort);
            }

            ClearConnections();
        }

        /// <summary> Swap connected nodes from the old list with nodes from the new list </summary>
        public void Redirect(List<UniBaseNode> oldNodes, List<UniBaseNode> newNodes)
        {
            foreach (var connection in connections) {
                var index                       = oldNodes.IndexOf(connection.node);
                if (index >= 0) connection.node = newNodes[index];
            }
        }
        
        #endregion

        #region private methods
        
        
        private void UpdateValueFilter(List<string> valueTypes)
        {
            portValueTypes = portValueTypes == null ? new List<Type>() : portValueTypes;
            portValueTypes.Clear();
            
            for (var i = 0; i < valueTypes.Count; i++) {
                var typeFilter = valueTypes[i];
                var type       = Type.GetType(typeFilter, false, true);
                if (type != null) portValueTypes.Add(type);
            }
        }
        
        #endregion
        
    }
}