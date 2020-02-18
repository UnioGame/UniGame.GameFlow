namespace UniGreenModules.UniNodeSystem.Runtime.Core
{
    using UniCore.Runtime.Interfaces;
    using UniRx;

    public interface IGraphData : INamedItem, IUnique
    {

        #region read operations
        
        Node GetNode(int id);

        NodePort GetPort(int portId);
        
        PortConnection GetConnection(int portId);

        IReadOnlyReactiveCollection<NodePort> GetPorts(int nodeId);

        IReadOnlyReactiveCollection<PortConnection> GetConnections(int portId);
        
        #endregion
        
        int GetUniqueId();
        
        int UpdateId(int oldId);

        IGraphData Add(IGraphItem graphItem);

        #region remove operations
        
        IGraphData Remove(int id);
        
        IGraphData RemoveNode(Node node);

        IGraphData RemovePort(NodePort port);

        IGraphData RemoveConnection(PortConnection connection);
        
        #endregion
    }
}