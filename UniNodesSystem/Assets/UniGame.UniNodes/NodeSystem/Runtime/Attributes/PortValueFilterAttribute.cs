using System;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Attributes
{
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using UniNodeSystem.Runtime.Core;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class PortValueFilterAttribute : Attribute, IPortData
    {
        public string portName;
        public PortIO direction;
        public ConnectionType connectionType;
        public ShowBackingValue backingValue;
        public List<Type> typeFilter;

        public PortValueFilterAttribute(
            string portName,
            PortIO direction = PortIO.Input, 
            ConnectionType connectionType = ConnectionType.Multiple,
            ShowBackingValue backingValue = ShowBackingValue.Always,
            params Type[] typeFilter)
        {
            this.portName = portName;
            this.direction = direction;
            this.connectionType = connectionType;
            this.backingValue = backingValue;
            this.typeFilter = typeFilter.ToList();
        }

        public string FieldName => portName;
        public PortIO Direction => direction;
        public ConnectionType ConnectionType => connectionType;
        
        public ShowBackingValue ShowBackingValue => backingValue;
        public bool InstancePortList { get; } = false;
        public IReadOnlyList<Type> ValueTypes => typeFilter;
    }
}
