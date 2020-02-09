namespace UniGreenModules.UniNodeSystem.Runtime.Interfaces
{
    using System.Collections.Generic;
    using UniStateMachine.Runtime.Interfaces;

    public interface IUniNode : 
        INode,
        IState
    {
        IReadOnlyCollection<IPortValue> PortValues { get; }

        bool AddPortValue(IPortValue portValue);
    }
}