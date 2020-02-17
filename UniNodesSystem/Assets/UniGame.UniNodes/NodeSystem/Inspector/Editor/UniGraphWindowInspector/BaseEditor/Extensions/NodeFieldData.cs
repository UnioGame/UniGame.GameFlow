namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor.Extensions
{
    using System;
    using System.Reflection;
    using UniNodeSystem.Runtime.Core;
    using UnityEngine;

    [Serializable]
    public struct NodeFieldData
    {
        [SerializeReference]
        public IPortData PortData;
        
        public object    Value;
        
        public FieldInfo FieldInfo;
    }
}