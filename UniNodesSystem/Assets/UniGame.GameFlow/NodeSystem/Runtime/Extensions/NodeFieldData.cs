namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor.Extensions
{
    using System;
    using System.Reflection;
    using Runtime.Core;
    using Runtime.Core.Interfaces;
    using UnityEngine;

    [Serializable]
    public struct NodeFieldData
    {
        [SerializeReference]
        public NodePortData PortData;
        
        public object    Value;
        
        public FieldInfo FieldInfo;
    }
}