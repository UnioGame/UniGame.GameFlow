using System.Collections.Generic;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Attributes
{
    using System;
    using Core;
    using Core.Interfaces;
    using UniNodeSystem.Runtime.Core;
    using UnityEditor;
    using UnityEngine;

    public class ReactivePortAttribute : PropertyAttribute , IReactivePortData
    {
        public bool                instancePortList = false;
        public string              fieldName        = string.Empty;
        public PortIO              direction        = PortIO.Output;
        public ConnectionType      connectionType   = ConnectionType.Multiple;
        public bool                isDynamic        = false;
        public ShowBackingValue    showBackingValue = ShowBackingValue.Always;
        public IReadOnlyList<Type> valueTypes;

        public ReactivePortAttribute(
            PortIO direction = PortIO.Input, 
            ConnectionType connectionType = ConnectionType.Multiple,
            ShowBackingValue backingValue = ShowBackingValue.Always)
        {
            this.direction        = direction;
            this.connectionType   = connectionType;
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
