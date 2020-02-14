namespace UniGreenModules.UniNodeSystem.Runtime.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Core;
    using UniCore.Runtime.Interfaces;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Interfaces;

    public interface INode : IImmutableNode
    {
  
        /// <summary> Remove an instance port from the node </summary>
        void RemovePort(string fieldName);

        /// <summary> Remove an instance port from the node </summary>
        void RemovePort(NodePort port);

        /// <summary> Disconnect everything from this node </summary>
        void ClearConnections();
        
        INodePort AddPort(
            string fieldName,
            IReadOnlyList<Type> types, PortIO direction,
            ConnectionType connectionType = ConnectionType.Multiple,
            ShowBackingValue showBackingValue = ShowBackingValue.Always);
    }
}