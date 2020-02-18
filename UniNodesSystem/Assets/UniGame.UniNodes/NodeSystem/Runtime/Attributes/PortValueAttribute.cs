namespace UniGame.UniNodes.NodeSystem.Runtime.Attributes
{
    using System;
    using System.Collections.Generic;
    using Core;
    using Core.Interfaces;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class PortValueAttribute : Attribute , IPortData
    {
        public bool             instancePortList = false;
        public string           fieldName        = string.Empty;
        public PortIO           direction        = PortIO.Output;
        public ConnectionType   connectionType   = ConnectionType.Multiple;
        public bool             isDynamic        = false;
        public ShowBackingValue showBackingValue = ShowBackingValue.Always;
        public IReadOnlyList<Type> valueTypes;

        public PortValueAttribute(
            PortIO direction = PortIO.Input, 
            ConnectionType connectionType = ConnectionType.Multiple,
            ShowBackingValue backingValue = ShowBackingValue.Always)
        {
            this.direction = direction;
            this.connectionType = connectionType;
            this.showBackingValue = backingValue;
        }
        
        public string ItemName => fieldName;

        public PortIO Direction => direction;

        public ConnectionType ConnectionType => connectionType;

        public bool Dynamic => isDynamic;

        public ShowBackingValue ShowBackingValue => showBackingValue;

        public bool InstancePortList => instancePortList;

        public IReadOnlyList<Type> ValueTypes => valueTypes;
    }
}
