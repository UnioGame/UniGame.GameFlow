namespace UniGreenModules.UniNodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core;

    public interface IPortData
    {
        string              ItemName        { get; }
        PortIO              Direction        { get; }
        ConnectionType      ConnectionType   { get; }
        ShowBackingValue    ShowBackingValue { get; }
        bool                InstancePortList { get; }
        IReadOnlyList<Type> ValueTypes       { get; }
    }
}