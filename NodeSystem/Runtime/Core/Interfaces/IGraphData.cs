﻿namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using Interfaces;
    using Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;

    public interface IGraphData : INamedItem, IUnique
    {
        int GetId();
        
        int UpdateId(int oldId);
        
        INode GetNode(int nodeId);

        IGraphData RemoveNode(INode node);

        IContext Context { get; }
    }
}