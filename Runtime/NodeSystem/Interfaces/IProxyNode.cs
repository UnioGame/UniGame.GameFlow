namespace UniModules.GameFlow.Runtime.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Core;
    using UniModules.UniGame.Core.Runtime.Interfaces;

    public interface IProxyNode : IUniNode
    {
        void Initialize(IGraphData graphData,
            Action initializeAction,
            Action<List<ILifeTimeCommand>> initializeCommands = null,
            Action executeAction = null);
    }
}