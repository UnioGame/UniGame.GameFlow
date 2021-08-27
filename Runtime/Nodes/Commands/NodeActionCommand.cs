namespace UniGame.UniNodes.Nodes.Runtime.Commands
{
    using System;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Core.Commands;
    using UniModules.GameFlow.Runtime.Extensions;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;

    [Serializable]
    public class NodeActionCommand<T> : ILifeTimeCommand
    {
        private PortActionCommand<T> portAction;

        public NodeActionCommand(
            Action<T> action, 
            IUniNode node, 
            string name, 
            PortIO direction = PortIO.Input)
        {
            var portInfo = node.UpdatePortValue(name, direction);
            portAction = new PortActionCommand<T>(action,portInfo);
        }
        
        public void Execute(ILifeTime lifeTime)
        {
            portAction.Execute(lifeTime);
        }
    }
}
