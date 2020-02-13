using UniGreenModules.UniNodeSystem.Runtime.Interfaces;
using UniRx;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core.Interfaces
{
    using System;

    public interface IReactiveSource
    {
        
        Type ValueType { get; }

        void Bind(IMessageBroker broker);
        
    }
}