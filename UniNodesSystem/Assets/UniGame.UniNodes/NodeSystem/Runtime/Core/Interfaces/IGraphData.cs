namespace UniGreenModules.UniNodeSystem.Runtime.Core
{
    using System.Collections.Generic;
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniCore.Runtime.Interfaces;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Interfaces;

    public interface IGraphData : INamedItem, IUnique
    {
        int GetId();
        
        Node GetNode(int id);

        int UpdateId(int oldId);

        //IGraphItem Add(IGraphItem graphItem);
        void RemoveNode(Node node);
    }
}