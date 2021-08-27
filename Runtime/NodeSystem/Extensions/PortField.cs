namespace UniModules.UniGameFlow.NodeSystem.Runtime.Extensions
{
    using System;
    using System.Reflection;
    using global::UniModules.GameFlow.Runtime.Core;
    using UnityEngine;

    [Serializable]
    public struct PortField
    {
        [SerializeReference]
        public PortData PortData;
        
        public object    Value;
        
        public FieldInfo FieldInfo;
    }
}