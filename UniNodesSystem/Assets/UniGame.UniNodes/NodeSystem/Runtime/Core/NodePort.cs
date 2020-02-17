namespace UniGreenModules.UniNodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using UniCore.Runtime.Attributes;
    using UniCore.Runtime.DataFlow;
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniCore.Runtime.ObjectPool.Runtime;
    using UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniCore.Runtime.ProfilerTools;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
    using UnityEngine;

    [Serializable]
    public class NodePort : INodePort, IPortData
    {
        #region inspector

        /// <summary>
        /// unique graph space port id 
        /// </summary>
        [ReadOnlyValue]
        [SerializeField] public int id;
        /// <summary>
        /// parent Node id
        /// </summary>
        [ReadOnlyValue]
        [SerializeField] public int nodeId;
        [SerializeField] public Node node;
        [SerializeField] public string           fieldName;
        [SerializeField] public PortIO           direction          = PortIO.Input;
        [SerializeField] public ConnectionType   connectionType     = ConnectionType.Multiple;
        [SerializeField] public ShowBackingValue showBackingValue   = ShowBackingValue.Always;
        [SerializeField] public bool             isDynamic          = true;
        [SerializeField] public bool             isInstancePortList = false;
        /// <summary>
        /// port container value
        /// </summary>
        [SerializeField] public UniPortValue portValue = new UniPortValue();
        /// <summary>
        /// registered port connections
        /// </summary>
        [SerializeField] public List<PortConnection> connections = new List<PortConnection>();
        [SerializeField] public bool instancePortList;
        
        #endregion

        [NonSerialized] public LifeTimeDefinition lifeTimeDefinition;
        /// <summary>
        /// port lifeTime 
        /// </summary>
        [NonSerialized] private ILifeTime lifeTime;
        /// <summary>
        /// draft validator refactoring. Move rule to SO files
        /// </summary>
        [NonSerialized] private IReadOnlyList<Func<NodePort, NodePort, bool>> connectionsValidators;

 
        #region constructor

        /// <summary>
        /// Copy a nodePort but assign it to another node.
        /// </summary>
        public NodePort(NodePort nodePort, Node node) :
            this(node,
                nodePort.ItemName,
                nodePort.Direction,
                nodePort.ConnectionType,
                nodePort.ShowBackingValue,
                nodePort.Value.ValueTypes)
        {
            connections.Clear();
            connections.AddRange(nodePort.connections);

        }

        public NodePort(Node node, IPortData portData) :
            this(node,
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
        public NodePort(Node node,
            string fieldName,
            Type type,
            PortIO direction = PortIO.Input,
            ConnectionType connectionType = ConnectionType.Multiple,
            ShowBackingValue showBackingValue = ShowBackingValue.Always) :
            this(node, fieldName, direction, connectionType, showBackingValue, new List<Type>() {type})
        {
        }


        public NodePort(
            Node node,
            string fieldName,
            PortIO direction = PortIO.Input,
            ConnectionType connectionType = ConnectionType.Multiple,
            ShowBackingValue showBackingValue = ShowBackingValue.Always,
            IReadOnlyList<Type> types = null)
        {

            this.fieldName        = fieldName;
            this.direction        = direction;
            this.connectionType   = connectionType;
            this.showBackingValue = showBackingValue;
            portValue.SetValueTypeFilter(types);

            Initialize(node);
            
            UpdateId();

        }

        #endregion

        #region public properties

        public IReadOnlyList<Func<NodePort, NodePort, bool>> ConnectionsValidators =>
            connectionsValidators ?? new List<Func<NodePort, NodePort, bool>>() {
                (source, to) => to != null && source != null,
                (source, to) => to != source,
                (source, to) => !source.IsConnectedTo(to),
                (source, to) => source.Direction != to.Direction,
                (source, to) =>
                    to.ValueTypes.Count == 0 || source.ValueTypes.Count == 0 ||
                    source.ValueTypes.Any(to.Value.IsValidPortValueType),
            };

        public int Id => id == 0 ? UpdateId() : id;

        public bool InstancePortList => instancePortList;

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

        public bool             IsInput          => Direction == PortIO.Input;
        public bool             IsOutput         => Direction == PortIO.Output;
        public string           ItemName         => fieldName;

        public ShowBackingValue ShowBackingValue => showBackingValue;
        public Type             ValueType        => portValue.ValueTypes.FirstOrDefault();

        public ILifeTime LifeTime => lifeTime;

        #endregion

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

        public void Initialize(Node data)
        {
            this.node = data;
            this.nodeId = data.Id;

            connections = connections ?? new List<PortConnection>();
            connections.ForEach(x => x.Initialize(node));

            //initialize port lifetime
            lifeTimeDefinition = lifeTimeDefinition ?? new LifeTimeDefinition();
            lifeTime           = lifeTimeDefinition.LifeTime;
            lifeTimeDefinition.Release();
            
            //initialize port value
            portValue.Initialize(fieldName);
            //bind port value to port lifetime
            lifeTime.AddDispose(portValue);
        }

        /// <summary>
        /// update related port id
        /// </summary>
        /// <param name="oldId"></param>
        /// <param name="newId"></param>
        /// <param name="updatedItem"></param>
        public void OnIdUpdate(int oldId, int newId, IGraphItem updatedItem)
        {
            if (nodeId == oldId) {
                nodeId = newId;
            }
            connections.ForEach(x => {
                if (x.nodeId == oldId)
                    x.nodeId = newId;
            });
        }

        
        /// <summary>
        /// terminate Port lifetime, release resources
        /// </summary>
        public void Release()
        {
            lifeTimeDefinition.Terminate();
        }

        #region node methods

        public Node Node => node;

        public int UpdateId()
        {
            var oldId = id;
            id = node.Graph.UpdateId(id);
            OnIdUpdate(oldId,id,this);
            return id;
        }

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
            removedConnections.DespawnCollection();
        }

        /// <summary> Connect this <see cref="NodePort"/> to another </summary>
        /// <param name="port">The <see cref="NodePort"/> to connect to</param>
        public void Connect(NodePort port)
        {
            if (connections == null) connections = new List<PortConnection>();

            if (!ConnectionsValidators.All(x => x(this, port))) {
                GameLog.LogError($"{node.Graph.ItemName}:{Node.ItemName}:{ItemName} Connection Error. Validation Failed");
                return;
            }

            if (port.ConnectionType == ConnectionType.Override && port.ConnectionCount != 0) {
                port.ClearConnections();
            }

            if (ConnectionType == ConnectionType.Override && ConnectionCount != 0) {
                ClearConnections();
            }

            var portNode = port.node;
            connections.Add(new PortConnection(port,portNode.id));

            if (port.connections == null)
                port.connections = new List<PortConnection>();
            if (!port.IsConnectedTo(this))
                port.connections.Add(new PortConnection(this,nodeId));
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
            if (i >= connections.Count || i < 0) return null;

            var connection = connections[i];
            //If the connection is broken for some reason, remove it.
            return connection.Port;
        }

        public void Validate()
        {
            if (id == 0) UpdateId();
            VerifyConnections();
        }

        /// <summary> Get index of the connection connecting this and specified ports </summary>
        public int GetConnectionIndex(NodePort port)
        {
            for (var i = 0; i < ConnectionCount; i++) {
                if (connections[i].portId == port.id) return i;
            }

            return -1;
        }

        public bool IsConnectedTo(NodePort port)
        {
            var result = false;
            for (var i = 0; i < connections.Count; i++) {
                if (connections[i].portId != port.id) {
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
                if (connection.portId == port.id) {
                    connections.RemoveAt(i);
                }
            }

            if (port != null) {
                // Remove the other ports connection to this port
                for (var i = 0; i < port.connections.Count; i++) {
                    var connection = port.connections[i];
                    if (connection.portId == id) {
                        port.connections.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary> Disconnect this port from another port </summary>
        public void Disconnect(int i)
        {
            // Remove the other ports connection to this port
            var otherPort = connections[i].GetPort();
            if (otherPort != null) {
                for (var k = 0; k < otherPort.connections.Count; k++) {
                    var connection = otherPort.connections[i];
                    if (connection.portId == id) {
                        otherPort.connections.RemoveAt(i);
                    }
                }
            }

            // Remove this ports connection to the other
            connections.RemoveAt(i);
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
        public void Redirect(List<Node> oldNodes, List<Node> newNodes)
        {
            foreach (var connection in connections) {
                var index = oldNodes.IndexOf(connection.Port.Node);
                if (index >= 0) {
                    var newNode = newNodes[index];
                    connection.Initialize(newNode,newNode.id,connection.fieldName); 
                }
            }
        }

        #endregion
    }
}