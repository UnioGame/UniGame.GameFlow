using UniGreenModules.UniNodeSystem.Runtime.Interfaces;
using UniRx;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core.Interfaces
{
    using System;
    using UniNodeSystem.Runtime.Core;

    public interface IReactiveSource
    {
        
        Type ValueType { get; }

        void Bind(NodeGraph graph,int nodeId, string portName);

    }
}