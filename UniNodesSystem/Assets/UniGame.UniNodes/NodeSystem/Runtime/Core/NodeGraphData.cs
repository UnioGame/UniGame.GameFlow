namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using UniGreenModules.UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniGame.Core.Runtime.DataStructure;
    using UnityEngine;

    [Serializable]
    public class NodesMap : SerializableDictionary<int, Node> {}

    [Serializable]
    public class ConnectionsMap : SerializableDictionary<int, List<PortConnection>> {}

    [Serializable]
    public class PortsMap : SerializableDictionary<int, NodePort> { }

    [Serializable]
    public class NodePortsMap : SerializableDictionary<int, List<NodePort>> {}

    [Serializable]
    public class NodeGraphData : IGraphData
    {
        /// <summary>
        /// draft validator refactoring. Move rule to SO files
        /// </summary>
        private IReadOnlyList<Func<NodePort, NodePort, bool>> connectionsValidators;
        protected IReadOnlyList<Func<NodePort, NodePort, bool>> ConnectionsValidators =>
            connectionsValidators ?? new List<Func<NodePort, NodePort, bool>>() {
                (source, to) => to != null && source != null,
                (source, to) => to != source,
                (source, to) => !source.IsConnectedTo(to),
                (source, to) => source.Direction != to.Direction,
                (source, to) =>
                    to.ValueTypes.Count == 0 || source.ValueTypes.Count == 0 ||
                    source.ValueTypes.Any(to.Value.IsValidPortValueType),
            };
    
        [SerializeField]
        private NodesMap nodesMap = new NodesMap();
    
        [SerializeField]
        private ConnectionsMap connectionsMap = new ConnectionsMap();
    
        [SerializeField]
        private PortsMap portsMap = new PortsMap();
    
        [SerializeField]
        private NodePortsMap nodePortsMap = new NodePortsMap();
    
        public string ItemName { get; }
    
        public int Id { get; }

        public Node GetNode(int id)
        {
            nodesMap.TryGetValue(id, out var node);
            return node;
        }

        public NodePort GetPort(int portId)
        {
            portsMap.TryGetValue(portId, out var port);
            return port;
        }

        public IReadOnlyList<NodePort> GetPorts(int nodeId)
        {
            if (nodePortsMap.TryGetValue(nodeId, out var ports)) {
                return ports;
            }

            ports                = new List<NodePort>();
            nodePortsMap[nodeId] = ports;
            return ports;
        }

        public IReadOnlyList<PortConnection> GetConnections(int portId)
        {
            if (connectionsMap.TryGetValue(portId, out var connections)) {
                return connections;
            }

            connections            = new List<PortConnection>();
            connectionsMap[portId] = connections;
            return connections;
        }

        public int GetUniqueId() => throw new NotImplementedException();

        public int UpdateId(int oldId) => throw new NotImplementedException();

        public IGraphData Add(IGraphItem graphItem)
        {
            switch (graphItem) {
                case Node node:
                    break;
                case NodePort port:
                    break;
            }

            return this;
        }

        public IGraphData ClearConnections(int portId)
        {
            if (connectionsMap.TryGetValue(portId, out var portConnections)) {
                portConnections.Clear();
            }

            return this;
        }
    
        public IGraphData AddConnection(int portFromId, int portToId)
        {
            var fromPort = GetPort(portFromId);
            var toPort   = GetPort(portToId);
            if (fromPort == null || toPort == null) {
                GameLog.LogError($"AddConnection ERROR: from {portFromId} to {portToId}");
                return this;
            } 
        
            if (!ConnectionsValidators.All(x => x(fromPort, toPort))) {
                GameLog.LogError($"AddConnection: {fromPort.ItemName} to {toPort.ItemName} Error. Validation Failed");
                return this;
            }

            var connections = GetConnections(portFromId);
            if (fromPort.ConnectionType == ConnectionType.Override && connections.Count != 0) {
                ClearConnections(portFromId);
            }

            connections = GetConnections(portToId);
            if (fromPort.ConnectionType == ConnectionType.Override && connections.Count != 0) {
                ClearConnections(portToId);
            }

            //todo connect
            //
            // var portNode = port.node;
            // connections.Add(new PortConnection(port,portNode.id));
            //
            // if (port.connections == null)
            //     port.connections = new List<PortConnection>();
            // if (!port.IsConnectedTo(this))
            //     port.connections.Add(new PortConnection(this,nodeId));
            return this;
        }

        public IGraphData Remove(int id) =>  this;

        public IGraphData RemoveNode(Node node) =>  this;

        public IGraphData RemovePort(NodePort port) =>  this;

        public IGraphData RemoveConnection(PortConnection connection) =>  this;
    }
}