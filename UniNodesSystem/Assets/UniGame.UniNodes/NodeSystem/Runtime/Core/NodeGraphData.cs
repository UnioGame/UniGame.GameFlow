using System;
using System.Collections.Generic;
using UniGreenModules.UniGame.Core.Runtime.DataStructure;
using UniGreenModules.UniNodeSystem.Runtime.Core;
using UniRx;
using UnityEngine;


[Serializable]
public class NodesMap : SerializableDictionary<int, Node> {}

[Serializable]
public class ConnectionsMap : SerializableDictionary<int, List<PortConnection>> {}

[Serializable]
public class PortsMap : SerializableDictionary<int, NodePort> { }

[Serializable]
public class NodeGraphData : IGraphData
{
    [SerializeField]
    private NodesMap nodesMap = new NodesMap();
    
    [SerializeField]
    private ConnectionsMap connectionsMap = new ConnectionsMap();
    
    
    public string ItemName { get; }
    
    public int Id { get; }
    
    public Node GetNode(int id) => throw new NotImplementedException();

    public NodePort GetPort(int portId) => throw new NotImplementedException();

    public PortConnection GetConnection(int portId) => throw new NotImplementedException();

    public IReadOnlyReactiveCollection<NodePort> GetPorts(int nodeId) => throw new NotImplementedException();

    public IReadOnlyReactiveCollection<PortConnection> GetConnections(int portId) => throw new NotImplementedException();

    public int GetUniqueId() => throw new NotImplementedException();

    public int UpdateId(int oldId) => throw new NotImplementedException();

    public IGraphData Add(IGraphItem graphItem) => throw new NotImplementedException();

    public IGraphData Remove(int id) => throw new NotImplementedException();

    public IGraphData RemoveNode(Node node) => throw new NotImplementedException();

    public IGraphData RemovePort(NodePort port) => throw new NotImplementedException();

    public IGraphData RemoveConnection(PortConnection connection) => throw new NotImplementedException();
}
