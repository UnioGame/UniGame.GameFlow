namespace UniGame.UniNodes.NodeSystem.Runtime.Core.Interfaces
{
    using System;
    using Runtime.Interfaces;

    public interface IReactiveSource
    {
        
        Type ValueType { get; }

        void Bind(INode target,string portName);

    }
}