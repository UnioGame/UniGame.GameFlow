namespace UniGame.UniNodes.NodeSystem.Runtime.Interfaces
{
    using System.Collections.Generic;
    using Core;
    using UniGreenModules.UniStateMachine.Runtime.Interfaces;

    public interface IUniNode : 
        INode,
        IState
    {
        IReadOnlyCollection<INodePort> PortValues { get; }

        bool AddPortValue(INodePort portValue);

        void Initialize(NodeGraph data);

    }
}