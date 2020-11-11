namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Runtime.Core;
    using Runtime.Core.Interfaces;
    using Runtime.Interfaces;
    using UniModules.UniCore.Runtime.ReflectionUtils;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Extensions;

    public static class PortNodeExtensions 
    {
        public static PortField GetPortData(this INode node, Type type, string fieldName)
        {
            var field = type.GetFieldInfoCached(fieldName);
            return node.GetPortData(field, fieldName);
        }

        public static PortField GetPortData(this INode node,FieldInfo info,string portName = "")
        {
            portName = string.IsNullOrEmpty(portName) ? info.Name : portName;
            
            var field    = info.FieldType;
            
            var portData = info.GetCustomAttributes(false).
                OfType<IPortData>().
                FirstOrDefault();
            
            if (portData == null) return new PortField() {
                FieldInfo = info
            };

            var name          = string.IsNullOrEmpty(portData.ItemName) ? portName : portData.ItemName;
            var direction     = portData.Direction;
            var showBackValue = portData.ShowBackingValue;
            var connection    = portData.ConnectionType;
        
            var typeData = portData.ValueTypes;
            var types    = new List<Type>() {field};
            if(typeData != null) {
                types.Clear();
                types.AddRange(portData.ValueTypes);
            }

            var value          = info.GetValue(node);
            var reactiveSource = value as IReactiveSource;
            if (reactiveSource!=null) {
                types.Clear();
                types.Add(reactiveSource.ValueType);
            }
            
            var result = new PortData() {
                direction        = direction,
                connectionType   = connection,
                fieldName        = name,
                isDynamic        = true,
                valueTypes       = types,
                instancePortList = false,
                showBackingValue = showBackValue,
            };
            
            return new PortField() {
                Value = value,
                FieldInfo = info,
                PortData = result,
            };
        }


    }
}
