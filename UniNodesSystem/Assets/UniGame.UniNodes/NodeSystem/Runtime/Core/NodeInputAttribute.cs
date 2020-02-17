namespace UniGreenModules.UniNodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core;

    /// <summary> Mark a serializable field as an input port. You can access this through <see cref="GetInputPort(string)"/> </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class NodeInputAttribute : Attribute,IPortData
    {

        public bool instancePortList;
        public string fieldName;
        public PortIO direction = PortIO.Input;
        public ConnectionType connectionType;
        public bool isDynamic = false;
        public ShowBackingValue showBackingValue;
        public IReadOnlyList<Type> valueTypes;

        /// <summary> Mark a serializable field as an input port. You can access this through <see cref="GetInputPort(string)"/> </summary>
        /// <param name="backingValue">Should we display the backing value for this port as an editor field? </param>
        /// <param name="connectionType">Should we allow multiple connections? </param>
        public NodeInputAttribute(
            ShowBackingValue backingValue = ShowBackingValue.Always,
            ConnectionType connectionType = ConnectionType.Multiple, 
            bool instancePortList = false)
        {
            this.showBackingValue = backingValue;
            this.connectionType = connectionType;
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