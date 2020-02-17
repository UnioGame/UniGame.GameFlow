namespace UniGreenModules.UniNodeSystem.Nodes.Commands
{
    using System;
    using Runtime.Core;
    using Runtime.Extensions;
    using Runtime.Interfaces;
    using UniCore.Runtime.DataFlow;
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniCore.Runtime.Interfaces;

    [Serializable]
    public class ConnectedFormatedPairCommand : ILifeTimeCommand, IPortPair
    {
        public IPortValue InputPort { get; protected set; }
        
        public IPortValue OutputPort { get; protected set; }
        
        public ConnectedFormatedPairCommand(
            IUniNode node, 
            string input, 
            bool connect = true)
        {
            var inputName = input.GetFormatedPortName(PortIO.Input);
            var outputName = input.GetFormatedPortName(PortIO.Output);
            
            var ports = node.CreatePortPair(inputName, outputName, connect);
            
            InputPort = ports.inputValue;
            OutputPort = ports.outputValue;
        }
        
        public void Execute(ILifeTime lifeTime) {}
        
    }
}
