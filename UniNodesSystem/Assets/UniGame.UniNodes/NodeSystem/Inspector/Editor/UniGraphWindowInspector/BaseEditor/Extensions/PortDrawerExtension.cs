using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core;
using UniGreenModules.UniNodeSystem.Runtime.Core;

public static class PortDrawerExtension
{
    public static NodePort CreatePortByAttributes(this FieldInfo fieldInfo)
    {
        var data = fieldInfo.GetPortData();
        var result = new NodePort(null, data);
        return result;
    }

    public static IPortData GetPortDataWithAttributes(this NodePort port, string property)
    {
        var portData = port.Node.GetType().
            GetPortDataByType(property);
        return portData ?? port;
    }
    
    public static IPortData GetPortDataByType(this Type target,string portName)
    {
        var field = target.GetField(portName);
        var result = field.GetPortData();
        return result;
    }

    public static IPortData GetPortData(this FieldInfo fieldInfo)
    {
        var field = fieldInfo;
        var items = field.
            GetCustomAttributes(false).
            ToList();

        var result = PortDataByObjects(items, fieldInfo.FieldType, fieldInfo.Name);
        return result;
    }

    public static IPortData PortDataByObjects(this IReadOnlyList<object> portDatas,Type targetType,string portName = "")
    {
        if (portDatas == null)
            return null;
        
        var items = portDatas.
            OfType<IPortData>().
            ToList();

        if (items.Count == 0) return null;


        var first         = items.FirstOrDefault();
        var nameData      = items.FirstOrDefault(x => string.IsNullOrEmpty(x.FieldName) == false);
        var name          = nameData == null ? portName : nameData.FieldName;
        var direction     = first.Direction;
        var showBackValue = first.ShowBackingValue;
        var connection    = first.ConnectionType;
        var typeData      = items.FirstOrDefault(x => x.ValueTypes != null);
        var types         = typeData == null ? new List<Type>() {targetType} : typeData.ValueTypes;
        
        var result = new NodePortData() {
            direction        = direction,
            connectionType   = connection,
            fieldName        = name,
            isDynamic        = true,
            valueTypes       = types,
            instancePortList = false,
            showBackingValue = showBackValue,
        };

        return result;
    }
    
}
