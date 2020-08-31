namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime.Interfaces;
    using UnityEngine;

    [Serializable]
    public class PortData : IPortData , IPoolable , ISerializationCallbackReceiver
    {
        public List<string> typesValues = new List<string>();
        public string fieldName;
        public PortIO direction = PortIO.Input;
        public ConnectionType connectionType = ConnectionType.Multiple;
        public bool isDynamic = true;
        public ShowBackingValue showBackingValue = ShowBackingValue.Always;
        public bool instancePortList = false;
        public List<Type> valueTypes;

        public string ItemName => fieldName;

        public PortIO Direction => direction;

        public ConnectionType ConnectionType => connectionType;

        public bool Dynamic => isDynamic;

        public ShowBackingValue ShowBackingValue => showBackingValue;

        public bool InstancePortList => instancePortList;

        public IReadOnlyList<Type> ValueTypes => valueTypes;

        public PortData() { }

        public PortData(IPortData portData)
        {
            fieldName = portData.ItemName;
            direction = portData.Direction;
            connectionType = portData.ConnectionType;
            showBackingValue = portData.ShowBackingValue;
            valueTypes = portData.ValueTypes.ToList();
        }
        
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

        public void OnBeforeSerialize()
        {
            typesValues = valueTypes.Select(x => x.AssemblyQualifiedName).ToList();
        }

        public void OnAfterDeserialize()
        {
            valueTypes = valueTypes ?? new List<Type>();
            valueTypes.Clear();
            valueTypes.AddRange(typesValues.
                Select(x => Type.GetType(x,false,false)).
                Where(x => x!=null));
        }
    }
}
