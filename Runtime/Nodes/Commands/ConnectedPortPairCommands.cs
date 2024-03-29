﻿namespace UniGame.UniNodes.Nodes.Runtime.Commands
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniModules.GameFlow.Runtime.Core.Interfaces;
    using UniModules.GameFlow.Runtime.Extensions;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.DataFlow.Interfaces;
    using Core.Runtime;

    [Serializable]
    public class ConnectedPortPairCommands : ILifeTimeCommand , IPortPair
    {
        protected IPortValue inputPort;
        protected IPortValue outputPort;
        
        public IPortValue InputPort  => inputPort;
        
        public IPortValue OutputPort => outputPort;
        
        public void Initialize(IUniNode node,
            string input, 
            string output, 
            bool connect = true)
        {
            var ports = node.CreatePortPair(input, output,connect);
            inputPort = ports.inputValue;
            outputPort = ports.outputValue;
        }
        
        public virtual UniTask Execute(ILifeTime lifeTime) => UniTask.CompletedTask;

    }
}
