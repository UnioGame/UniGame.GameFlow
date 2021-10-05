namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using System;
    using System.Collections.Generic;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Core.Nodes;
    using UniModules.GameFlow.Runtime.Extensions;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;

    [HideNode]
    [Serializable]
    public class OutPortNode : SNode
    {
        public const string OutputPortName = "output";

        public IPortValue OutputPort => GetPortValue(OutputPortName);

        public override string GetStyle() => "GameFlow/UCSS/OutputPortNodeStyle";

        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);
            this.UpdatePortValue(OutputPortName, PortIO.Output);
        }
    }
}