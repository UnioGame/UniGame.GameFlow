namespace UniModules.GameFlow.Runtime.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Core;
    using Cysharp.Threading.Tasks;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniRx;

    public interface IProxyNode : IUniNode
    {
        void Initialize(NodeGraph graphData,
            Action initializeAction,
            Action<List<ILifeTimeCommand>> initializeCommands = null,
            Func<UniTask>  executeAction = null);
    }
}