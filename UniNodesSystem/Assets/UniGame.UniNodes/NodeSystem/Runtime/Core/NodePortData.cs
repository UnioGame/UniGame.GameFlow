namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using UniCore.Runtime.ObjectPool.Runtime.Interfaces;
    using UniNodeSystem.Runtime.Core;

    [Serializable]
    public class NodePortData : IPortData , IPoolable
    {
        public string fieldName;
        public PortIO direction = PortIO.Input;
        public ConnectionType connectionType = ConnectionType.Multiple;
        public bool isDynamic = true;
        public ShowBackingValue showBackingValue = ShowBackingValue.Always;
        public bool instancePortList = false;
        public IReadOnlyList<Type> valueTypes;

        public string FieldName => fieldName;

        public PortIO Direction => direction;

        public ConnectionType ConnectionType => connectionType;

        public bool Dynamic => isDynamic;

        public ShowBackingValue ShowBackingValue => showBackingValue;

        public bool InstancePortList => instancePortList;

        public IReadOnlyList<Type> ValueTypes => valueTypes;
        
        public void Release()
        {
            fieldName = string.Empty;
            direction = PortIO.Input;
            connectionType = ConnectionType.Multiple;
            isDynamic = true;
            showBackingValue = ShowBackingValue.Always;
            instancePortList = false;
            valueTypes = null;
        }
    }
}
