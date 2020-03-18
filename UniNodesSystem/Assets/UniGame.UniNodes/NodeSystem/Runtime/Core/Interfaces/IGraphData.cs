namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using Interfaces;
    using Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;

    public interface IGraphData : INamedItem, IUnique
    {
        int UpdateId(int oldId);
        
        INode GetNode(int nodeId);

        IGraphData RemoveNode(INode node);

//        IGraphData AddItem(IGraphItem item);
//
//        IGraphItem Get(int id);
    }
}