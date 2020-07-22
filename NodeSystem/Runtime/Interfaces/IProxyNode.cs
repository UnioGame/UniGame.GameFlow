namespace UniGame.UniNodes.NodeSystem.Runtime.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Core;
    using UniGreenModules.UniCore.Runtime.Interfaces;

    public interface IProxyNode : IUniNode
    {
        void Initialize(IGraphData graphData,
            Action initializeAction,
            Action<List<ILifeTimeCommand>> initializeCommands = null,
            Action executeAction = null);
    }
}