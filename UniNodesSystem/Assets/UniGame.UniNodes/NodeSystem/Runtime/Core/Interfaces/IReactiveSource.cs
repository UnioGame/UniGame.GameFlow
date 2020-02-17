using UniGreenModules.UniNodeSystem.Runtime.Interfaces;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core.Interfaces
{
    using System;

    public interface IReactiveSource
    {
        
        Type ValueType { get; }

        void Bind(INode target,string portName);

    }
}