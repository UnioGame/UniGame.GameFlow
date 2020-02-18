namespace UniGame.UniNodes.Nodes.Runtime.Commands
{
    using System;
    using NodeSystem.Runtime.Extensions;
    using NodeSystem.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;

    [Serializable]
    public class ConnectedPortPairCommands : ILifeTimeCommand
    {
        protected IPortValue inputPort;
        protected IPortValue outputPort;

        public void Initialize(IUniNode node,
            string input, 
            string output, bool connect = true)
        {
            var ports = node.CreatePortPair(input, output,connect);
            inputPort = ports.inputValue;
            outputPort = ports.outputValue;
        }
        
        public virtual void Execute(ILifeTime lifeTime){}

    }
}
