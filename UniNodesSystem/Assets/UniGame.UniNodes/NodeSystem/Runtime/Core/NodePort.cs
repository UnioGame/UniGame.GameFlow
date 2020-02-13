namespace UniGreenModules.UniNodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Interfaces;
    using UniCore.Runtime.Attributes;
    using UniCore.Runtime.DataFlow;
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniCore.Runtime.ObjectPool.Runtime;
    using UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniCore.Runtime.ProfilerTools;
    using UniCore.Runtime.Rx.Extensions;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
    using UniRx;
    using UnityEngine;

    [Serializable]
    public class NodePort : INodePort , IPortData
    {
        #region inspector

        /// <summary>
        /// unique graph space port id 
        /// </summary>
        [ReadOnlyValue]
        [SerializeField] protected ulong _id;
        [SerializeField] protected string               _fieldName;
        [SerializeField] protected PortIO               _direction = PortIO.Input;
        [SerializeField] protected ConnectionType       _connectionType = ConnectionType.Multiple;
        [SerializeField] protected ShowBackingValue     _showBackingValue = ShowBackingValue.Always;
        [SerializeField] protected bool isDynamic = true;
        [SerializeField] protected bool isInstancePortList = false;
        
        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineEditor()]
#endif
        protected UniBaseNode _node;

        /// <summary>
        /// port container value
        /// </summary>
        [SerializeField] protected UniPortValue _portValue = new UniPortValue();

        /// <summary>
        /// registered port connections
        /// </summary>
        [SerializeField] protected List<PortConnection> connections = new List<PortConnection>();

        #endregion

        private LifeTimeDefinition _lifetime;

        /// <summary>
        /// draft validator refactoring. Move rule to SO files
        /// </summary>
        private IReadOnlyList<Func<NodePort, NodePort, bool>> _connectionsValidators;
        
        private bool instancePortList;

        #region constructor

        /// <summary>
        /// Copy a nodePort but assign it to another node.
        /// </summary>
        public NodePort(NodePort nodePort, UniBaseNode node) :
            this(node,
                nodePort.FieldName,
                nodePort.Direction,
                nodePort.ConnectionType,
                nodePort.ShowBackingValue,
                nodePort.Value.ValueTypes)
        {
            GameLog.Log("NodePort FROM NODE");
            _node = node;

            connections.Clear();
            connections.AddRange(nodePort.connections);

            UpdateId();
            
        }

        public NodePort(UniBaseNode node,IPortData portData) : 
            this(node,
                portData.FieldName,
                portData.Direction,
                portData.ConnectionType,
                portData.ShowBackingValue,
                portData.ValueTypes) { }
        
        /// <summary>
        /// Construct a dynamic port. Dynamic ports are not forgotten on reimport,
        /// and is ideal for runtime-created ports.
        /// </summary>
        public NodePort(UniBaseNode node,
            string fieldName,
            Type type,
            PortIO direction = PortIO.Input,
            ConnectionType connectionType = ConnectionType.Multiple,
            ShowBackingValue showBackingValue = ShowBackingValue.Always) :
            this(node,fieldName, direction, connectionType, showBackingValue,new List<Type>() {type})
        {
        }


        public NodePort(
            UniBaseNode node,
            string fieldName,
            PortIO direction = PortIO.Input,
            ConnectionType connectionType = ConnectionType.Multiple,
            ShowBackingValue showBackingValue = ShowBackingValue.Always,
            IReadOnlyList<Type> types = null)
        {
            _fieldName      = fieldName;
            _direction      = direction;
            _node           = node;
            _connectionType = connectionType;
            _showBackingValue = showBackingValue;
            
            _portValue.SetValueTypeFilter(types);

            UpdateId();
            
            Initialize();
        }

        #endregion

        #region public properties

        public IReadOnlyList<Func<NodePort, NodePort, bool>> ConnectionsValidators =>
            _connectionsValidators ?? new List<Func<NodePort, NodePort, bool>>() {
                (source, to) => to != null && source != null,
                (source, to) => to != source,
                (source, to) => !source.IsConnectedTo(to),
                (source, to) => source.Direction != to.Direction,
                (source, to) => 
                    to.ValueTypes.Count == 0 || source.ValueTypes.Count == 0 ||
                    source.ValueTypes.Any(to.Value.IsValidPortValueType),
            };

        public ulong Id => _id = _id == 0 ? UpdateId() : _id;

        public bool InstancePortList => instancePortList;

        public IReadOnlyList<Type> ValueTypes => _portValue.ValueTypes;

        public bool Dynamic => isDynamic;

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

        public IPortValue Value => _portValue;

        public PortIO         Direction      => _direction;
        public ConnectionType ConnectionType => _connectionType;

        /// <summary> Is this port connected to anytihng? </summary>
        public bool IsConnected => connections.Count != 0;

        public bool IsInput  => Direction == PortIO.Input;
        public bool IsOutput => Direction == PortIO.Output;

        public string FieldName => _fieldName;

        public string ItemName => _fieldName;
        
        public ShowBackingValue ShowBackingValue => _showBackingValue;

        public UniBaseNode Node {
            get => _node;
            set {
                if (_node == value)
                    return;
                _node = value;
                UpdateId();
            }
        }

        public Type ValueType => _portValue.ValueTypes.FirstOrDefault();

        public ILifeTime LifeTime => _lifetime.LifeTime;

        #endregion

        #region port value methods

        public void SetPortData(IPortData portData)
        {
            _fieldName = portData.FieldName;
            _direction = portData.Direction;
            _connectionType = portData.ConnectionType;
            _showBackingValue = portData.ShowBackingValue;
            
            _portValue.SetValueTypeFilter(portData.ValueTypes);
        }
        
        public void Dispose() => Release();

        public IObservable<Unit> PortValueChanged => _portValue.PortValueChanged;
        
        public bool HasValue => _portValue.HasValue;

        public TData Get<TData>() => _portValue.Get<TData>();

        public bool Remove<TData>() => _portValue.Remove<TData>();

        public bool Contains<T>() => _portValue.Contains<T>();

        public void Publish<T>(T message) => _portValue.Publish(message);

        public IDisposable Bind(IMessagePublisher publisher) => _portValue.Bind(publisher);

        public IObservable<T> Receive<T>() => GetObservable<T>();

        /// <summary>
        /// is target type value valid for this port
        /// </summary>
        public bool IsValidPortValueType(Type targetType) => _portValue.IsValidPortValueType(targetType);

        #endregion

        public void Initialize()
        {
            _lifetime = _lifetime ?? new LifeTimeDefinition();
            _lifetime.Release();
            _portValue.Initialize(_fieldName, _lifetime.LifeTime);
            
            BindToConnectedSources(_portValue);
        }

        public void Release()
        {
            _lifetime.Terminate();
        }

        #region node methods

        public ulong UpdateId() => _id = _node.Graph.GetId();

        /// <summary> Checks all connections for invalid references, and removes them. </summary>
        public void VerifyConnections()
        {
            var removedConnections = ClassPool.Spawn<List<PortConnection>>();
            
            for (var i = connections.Count - 1; i >= 0; i--) {
                var connection = connections[i];
                var targetPort = connection.node?.GetPort(connection.fieldName);
                
                if (targetPort != null)
                    continue;
                removedConnections.Add(connection);
            }
            
            removedConnections.ForEach(x => connections.Remove(x));
            removedConnections.DespawnCollection();
        }

        /// <summary> Connect this <see cref="NodePort"/> to another </summary>
        /// <param name="port">The <see cref="NodePort"/> to connect to</param>
        public void Connect(NodePort port)
        {
            if (connections == null) connections = new List<PortConnection>();

            if (!ConnectionsValidators.All(x => x(this, port))) {
                GameLog.LogError($"{_node.Graph.name}:{_node.name}:{FieldName} Connection Error. Validation Failed");
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
            
            VerifyConnections();

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
            var result = false;
            for (var i = 0; i < connections.Count; i++) {
                if (connections[i].Port.Id != port.Id) {
                    continue;
                }
                result = true;
                break;
            }
            
            return result;
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

        /// <summary>
        /// TODO MOVE TO  GRAPH RULES SO
        /// </summary>
        /// <returns></returns>
        private IReadOnlyList<Func<NodePort, NodePort, bool>> CreateConnectionValidators()
        {
            var validators = new List<Func<NodePort, NodePort, bool>>() {
                (source, to) => to != null && source != null,
                (source, to) => to != source,
                (source, to) => !source.IsConnectedTo(to),
                (source, to) => source.Direction != to.Direction,
                (source, to) => source.ValueTypes.Any(to.Value.IsValidPortValueType),
            };
            return validators;
        }

        /// <summary>
        /// Create connection between valid nodes 
        /// </summary>
        /// <typeparam name="TValue">Target connection</typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IObservable<TValue> GetObservable<TValue>()
        {
            return _portValue.Receive<TValue>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BindToConnectedSources(IMessagePublisher publisher)
        {
            //data source connections allowed only for input ports
            if (_direction != PortIO.Input) {
                return;
            }

            for (var i = 0; i < connections.Count; i++) {
                var connection = connections[i];
                var port       = connection.Port;
                if(port.Direction == PortIO.Input || port.Id == Id)
                    continue;
                connection.Port.Bind(publisher).
                    AddTo(LifeTime);
            }
        }

        #endregion
    }
}