namespace UniGreenModules.UniNodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core;

    /// <summary> Mark a serializable field as an output port. You can access this through <see cref="GetOutputPort(string)"/> </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class NodeOutputAttribute : Attribute, IPortData
    {
        public bool                instancePortList = false;
        public string              fieldName        = string.Empty;
        public PortIO              direction        = PortIO.Output;
        public ConnectionType      connectionType   = ConnectionType.Multiple;
        public bool                isDynamic        = false;
        public ShowBackingValue    showBackingValue = ShowBackingValue.Always;
        public IReadOnlyList<Type> valueTypes;

        /// <summary> Mark a serializable field as an output port. You can access this through <see cref="GetOutputPort(string)"/> </summary>
        /// <param name="backingValue">Should we display the backing value for this port as an editor field? </param>
        /// <param name="connectionType">Should we allow multiple connections? </param>
        public NodeOutputAttribute(
            ShowBackingValue backingValue = ShowBackingValue.Always,
            ConnectionType connectionType = ConnectionType.Multiple,
            bool instancePortList = false)
        {
            this.showBackingValue = backingValue;
            this.connectionType   = connectionType;
            this.instancePortList = instancePortList;
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