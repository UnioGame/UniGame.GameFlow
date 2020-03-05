namespace UniGame.UniNodes.NodeSystem.Runtime.Interfaces
{
    using System;
    using System.Collections.Generic;
    using UniGreenModules.UniCore.Runtime.Interfaces;

    public interface IProxyNode : IUniNode
    {
        void Bind(Action initializeAction = null,
            Action<List<ILifeTimeCommand>> initializeCommands = null,
            Action executeAction = null);
    }
}