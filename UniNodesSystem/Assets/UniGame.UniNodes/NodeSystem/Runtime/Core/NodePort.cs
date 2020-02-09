namespace UniGreenModules.UniNodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using UniCore.Runtime.Attributes;
    using UniCore.Runtime.DataFlow;
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniCore.Runtime.ObjectPool.Runtime;
    using UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniCore.Runtime.ProfilerTools;
    using UniRx;
    using UnityEngine;

    [Serializable]
    public class NodePort : INodePort
    {
        #region inspector

        /// <summary>
        /// unique graph space port id 
        /// </summary>
        [ReadOnlyValue]
        [SerializeField] private ulong _id;
        [SerializeField] protected string               _fieldName;
        [SerializeField] protected UniBaseNode          _node;
        [SerializeField] protected List<PortConnection> connections = new List<PortConnection>();
        [SerializeField] protected PortIO               _direction;
        [SerializeField] protected ConnectionType       _connectionType;
        [SerializeField] protected bool                 _dynamic;

        /// <summary>
        /// allowed port value types
        /// </summary>
        [SerializeField] protected List<string> _allowedValueTypes;


        /// <summary>
        /// port container value
        /// </summary>
        [SerializeField] protected UniPortValue _portValue = new UniPortValue();

        #endregion

        private LifeTimeDefinition _lifetime = new LifeTimeDefinition();

        private List<Type> _portValueTypes = new List<Type>();

        private HashSet<Type> _portValueSubscriptions = new HashSet<Type>();

        /// <summary>
        /// draft validator refactoring. Move rule to SO files
        /// </summary>
        private IReadOnlyList<Func<NodePort, NodePort, bool>> _connectionsValidators;

        #region  constructor

        /// <summary>
        /// Construct a static targetless nodeport. Used as a template.
        /// </summary>
        public NodePort(FieldInfo fieldInfo)
        {
            _fieldName = fieldInfo.Name;
            _dynamic   = false;

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

            SetValueFilter(new List<Type>() {fieldInfo.FieldType});
            Initialize();
        }

        /// <summary>
        /// Copy a nodePort but assign it to another node.
        /// </summary>
        public NodePort(NodePort nodePort, UniBaseNode node) :
            this(nodePort.FieldName,
                nodePort.Direction,
                nodePort.ConnectionType,
                node,
                nodePort.ValueTypes)
        {
        }

        /// <summary>
        /// Construct a dynamic port. Dynamic ports are not forgotten on reimport,
        /// and is ideal for runtime-created ports.
        /// </summary>
        public NodePort(string fieldName,
            Type type,
            PortIO direction,
            ConnectionType connectionType,
            UniBaseNode node) :
            this(fieldName, direction, connectionType, node, new List<Type>() {type})
        {
        }


        public NodePort(
            string fieldName,
            PortIO direction,
            ConnectionType connectionType,
            UniBaseNode node,
            IReadOnlyList<Type> types)
        {
            _fieldName      = fieldName;
            _direction      = direction;
            _node           = node;
            _dynamic        = true;
            _connectionType = connectionType;

            SetValueFilter(types);
            Initialize();
        }

        #endregion

        #region public properties

        public IReadOnlyList<Func<NodePort, NodePort, bool>> ConnectionsValidators =>
            _connectionsValidators == null
                ? new List<Func<NodePort, NodePort, bool>>() {
                    (source, to) => to != null && source != null,
                    (source, to) => to != source,
                    (source, to) => !source.IsConnectedTo(to),
                    (source, to) => source.Direction != to.Direction,
                    (source, to) => source.ValueTypes.Any(to.ValidateValueType),
                }
                : _connectionsValidators;
        
        public ulong Id => _id = _id == 0 ? UpdateId() : _id;

        public IReadOnlyList<Type> ValueTypes => _portValueTypes;

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

        public PortIO         Direction      => _direction;
        public ConnectionType ConnectionType => _connectionType;

        /// <summary> Is this port connected to anytihng? </summary>
        public bool IsConnected => connections.Count != 0;

        public bool IsInput  => Direction == PortIO.Input;
        public bool IsOutput => Direction == PortIO.Output;

        public string FieldName => _fieldName;

        public string ItemName => _fieldName;

        public bool IsDynamic => _dynamic;

        public bool IsStatic => !_dynamic;

        public UniBaseNode Node {
            get => _node;
            set {
                if (_node == value)
                    return;
                _node = value;
                _id   = _node.Graph.GetId();
            }
        }

        public Type ValueType => ValueTypes.FirstOrDefault();
        
        public bool HasValue => _portValue.HasValue;

        public ILifeTime LifeTime => _lifetime.LifeTime;

        #endregion

        #region port value methods

        public void Dispose() => Release();

        public IObservable<Unit> PortValueChanged => _portValue.PortValueChanged;

        public TData Get<TData>() => _portValue.Get<TData>();

        public bool Remove<TData>() => _portValue.Remove<TData>();
        
        public bool Contains<T>() => _portValue.Contains<T>();

        public void Publish<T>(T message) => _portValue.Publish(message);

        public IObservable<T> Receive<T>() => GetObservable<T>();

        public IDisposable Connect(IMessagePublisher publisher) => _portValue.Connect(publisher);

        public void SetValueFilter(IReadOnlyList<Type> allowedTypes)
        {
            _portValueTypes = _portValueTypes == null ? new List<Type>() : _portValueTypes;
            _portValueTypes.Clear();
            _portValueTypes.AddRange(allowedTypes);

            UpdateSerializedFilter(allowedTypes);
        }

        public bool ValidateValueType<T>()
        {
            return ValidateValueType(typeof(T));
        }

        /// <summary>
        /// is target type value valid for this port
        /// </summary>
        public bool ValidateValueType(Type targetType)
        {
            if (_portValueTypes == null || _portValueTypes.Count == 0) 
                return true;
            
            for (var i = 0; i < _portValueTypes.Count; i++) {
                var type = _portValueTypes[i];
                if (type == targetType) return true;
            }

            return false;
        }

        #endregion

        public void Initialize()
        {
            _lifetime = _lifetime ?? new LifeTimeDefinition();
            _lifetime.Release();
            
            _portValue.Initialize(_fieldName, _lifetime.LifeTime, ValidateValueType);
            
            InitializeTypeFilter();
        }

        public void Release()
        {
            _lifetime.Terminate();
            _portValueSubscriptions.Clear();
        }

        #region node methods

        public ulong UpdateId() => _id = _node.Graph.GetId();

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

            if (!ConnectionsValidators.All(x => x(this, port))) {
                GameLog.LogError("Port Connection Error");
                return;
            }

            if (port.ConnectionType == ConnectionType.Override && port.ConnectionCount != 0) {
                port.ClearConnections();
            }

            if (ConnectionType == ConnectionType.Override && ConnectionCount != 0) {
                ClearConnections();
            }

            connections.Add(new PortConnection(port));
            if (port.connections == null) port.connections = new List<PortConnection>();
            if (!port.IsConnectedTo(this)) port.connections.Add(new PortConnection(this));
            Node.OnCreateConnection(this, port);
            port.Node.OnCreateConnection(this, port);
        }

        public List<INodePort> GetConnections()
        {
            var result = ClassPool.Spawn<List<INodePort>>();
            for (var i = 0; i < connections.Count; i++) {
                var port = GetConnection(i);
                if (port != null) result.Add(port);
            }

            return result;
        }

        public INodePort GetConnection(int i)
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
            Node.OnRemoveConnection(this);
            port?.Node.OnRemoveConnection(port);
        }

        /// <summary> Disconnect this port from another port </summary>
        public void Disconnect(int i)
        {
            // Remove the other ports connection to this port
            var otherPort = connections[i].GetPort();
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
            Node.OnRemoveConnection(this);
            otherPort?.Node.OnRemoveConnection(otherPort);
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

        private IReadOnlyList<Func<NodePort, NodePort, bool>> CreateConnectionValidators()
        {
            var validators = new List<Func<NodePort, NodePort, bool>>() {
                (source, to) => to != null && source!=null,
                (source, to) => to != source,
                (source, to) => !source.IsConnectedTo(to),
                (source, to) => source.Direction != to.Direction,
                (source, to) => source.ValueTypes.Any(to.ValidateValueType),
            };
            return validators;
        }
        
        private void InitializeTypeFilter()
        {
            _portValueTypes = _portValueTypes ?? new List<Type>();
            _portValueTypes.Clear();
            _portValueTypes.AddRange(_allowedValueTypes.Select(x => Type.GetType(x, false)).Where(x => x != null).ToList());
        }

        
        
        /// <summary>
        /// Create connection between valid nodes 
        /// </summary>
        /// <typeparam name="TValue">Target connection</typeparam>
        /// <returns></returns>
        private IObservable<TValue> GetObservable<TValue>()
        {
            _portValueSubscriptions = _portValueSubscriptions ?? new HashSet<Type>();

            var type = typeof(TValue);
            if (_portValueSubscriptions.Add(type))
            {
                for (var i = 0; i < connections.Count; i++) {
                    var connection     = connections[i];
                    connection.Port.Connect(this);
                }
            }

            return _portValue.Receive<TValue>();
        }

        #region Unity Methods

        [Conditional("UNITY_EDITOR")]
        private void UpdateSerializedFilter(IReadOnlyList<Type> filter)
        {
            UpdateValueFilter(filter.Select(x => x.AssemblyQualifiedName).ToList());
        }

        [Conditional("UNITY_EDITOR")]
        private void UpdateValueFilter(List<string> valueTypes)
        {
            _allowedValueTypes = _allowedValueTypes ?? new List<string>();
            _allowedValueTypes.Clear();

            for (var i = 0; i < valueTypes.Count; i++) {
                var typeFilter = valueTypes[i];
                var type       = Type.GetType(typeFilter, false, true);
                if (type != null)
                    _allowedValueTypes.Add(typeFilter);
            }
        }

        #endregion

        #endregion

    }
}