namespace UniGreenModules.UniNodeSystem.Runtime.Core
{
    using System.Collections.Generic;
    using UniCore.Runtime.Interfaces;
    using UniRx;

    public interface IGraphData : INamedItem, IUnique
    {
        #region read operations
        
        Node GetNode(int id);

        NodePort GetPort(int portId);

        IReadOnlyList<NodePort> GetPorts(int nodeId);

        IReadOnlyList<PortConnection> GetConnections(int portId);
        
        #endregion
        
        int GetUniqueId();
        
        int UpdateId(int oldId);

        IGraphData Add(IGraphItem graphItem);

        IGraphData AddConnection(int portFromId, int portToId);
        
        
        
        #region remove operations
        
        IGraphData Remove(int id);
        
        IGraphData RemoveNode(Node node);

        IGraphData RemovePort(NodePort port);

        IGraphData RemoveConnection(PortConnection connection);
        
        #endregion
    }
}