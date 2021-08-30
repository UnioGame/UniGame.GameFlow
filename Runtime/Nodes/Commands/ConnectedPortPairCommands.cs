namespace UniGame.UniNodes.Nodes.Runtime.Commands
{
    using System;
    using UniModules.GameFlow.Runtime.Core.Interfaces;
    using UniModules.GameFlow.Runtime.Extensions;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;

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
        
        public virtual void Execute(ILifeTime lifeTime){}

    }
}
