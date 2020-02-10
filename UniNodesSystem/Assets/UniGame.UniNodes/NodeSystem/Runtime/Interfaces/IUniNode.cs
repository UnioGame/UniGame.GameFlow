namespace UniGreenModules.UniNodeSystem.Runtime.Interfaces
{
    using System.Collections.Generic;
    using Core;
    using UniStateMachine.Runtime.Interfaces;

    public interface IUniNode : 
        INode,
        IState
    {
        IReadOnlyCollection<INodePort> PortValues { get; }

        bool AddPortValue(INodePort portValue);
    }
}