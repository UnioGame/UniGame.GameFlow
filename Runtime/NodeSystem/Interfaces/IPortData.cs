namespace UniModules.GameFlow.Runtime.Core.Interfaces
{
    using System;
    using System.Collections.Generic;

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