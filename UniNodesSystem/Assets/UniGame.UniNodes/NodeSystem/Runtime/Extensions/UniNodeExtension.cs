namespace UniGame.UniNodes.NodeSystem.Runtime.Extensions
{
    using System;
    using System.Collections.Generic;
    using Core;
    using Core.Interfaces;
    using Interfaces;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniCore.Runtime.Utils;
    using UniRx;

    public static class UniNodeExtension
    {
        private const int InputNameIndex = 0;
        private const int OutputNameIndex = 1;

        public const string InputPattern = @"(\[?[\w\num ]*\])";
        
        public const string InputTriggerPrefix  = "[in]";
        public const string OutputTriggerPrefix = "[out]";       
        
        public static Func<string, string[]> portNameCache = MemorizeTool.Create((string x) => new string[2]);
   
#region port names
        
        private static string GetFormatedInputPortName(this string portName)
        {
            portName = string.Format($"{InputTriggerPrefix}{portName}");
            return portName;
        }

        private static string GetFormatedOutputPortName(this string portName)
        {
            portName = string.Format($"{OutputTriggerPrefix}{portName}");
            return portName;
        }
        
        public static string GetFormatedPortName(this string portName, PortIO direction)
        {
            var names = portNameCache(portName);

            var index = direction == PortIO.Input ? InputNameIndex: OutputNameIndex;
            var name = names[index];

            if (!string.IsNullOrEmpty(name)) {
                return name;
            }

            name = direction == PortIO.Input ? 
                GetFormatedInputPortName(portName) : 
                GetFormatedOutputPortName(portName);
            names[index] = name;

            return name;
        }
        
#endregion

        public static (IPortValue inputValue, IPortValue outputValue) 
            CreatePortPair(this IUniNode node,string inputPortName, string outputPortName, bool connectInOut = false)
        {
            var outputPort = node.UpdatePortValue(outputPortName, PortIO.Output);
            var inputPort  = node.UpdatePortValue(inputPortName, PortIO.Input);
                
            var inputValue  = inputPort;
            var outputValue = outputPort;
            
            if(connectInOut)
                inputValue.Bind(outputValue);
        
            return (inputValue,outputValue);
        }

        public static TValue GetConnectedNode<TValue>(this INodePort port)
            where TValue :Node
        {
            if (port == null || !port.IsConnected)
            {
                return null;
            }
            if (!(port.Node is TValue item))
            {
                return null;
            }

            return item;
        }
        
        public static void RegisterPortHandler<TValue>(
            this IUniNode node,
            IPortValue portValue,
            IObserver<TValue> observer,
            bool oneShot = false)
        {
            //subscribe to port value observable
            portValue.Receive<TValue>().
                Finally(() => {
                    //if node stoped or 
                    if (!oneShot || !node.IsActive) return;
                    //resubscribe to port values
                    node.RegisterPortHandler(portValue, observer, true);
                }).
                Subscribe(observer). //subscribe to port value changes
                AddTo(node.LifeTime);     //stop all subscriptions when node deactivated
        }

        
        public static IPortValue UpdatePortValue(this IUniNode node , IPortData portData)
        {
            if (portData == null)
                return null;
            
            var port = node.UpdatePortValue(
                portData.ItemName, 
                portData.Direction, 
                portData.ConnectionType,
                ShowBackingValue.Always, 
                portData.ValueTypes);
            
            
            return port;
        }

        public static IPortValue UpdatePortValue(
            this IUniNode node,
            string portName,
            PortIO direction = PortIO.Output,
            ConnectionType connectionType = ConnectionType.Multiple, 
            ShowBackingValue showBackingValue = ShowBackingValue.Always,
            IReadOnlyList<Type> types = null)
        {
            var port = node.GetPort(portName);

            if (port == null) {
                types = types ?? new List<Type>();
                port = node.AddPort(portName, types, direction, connectionType, showBackingValue);
            }

            var portData = new NodePortData() {
                direction        = direction,
                fieldName        = portName,
                connectionType   = connectionType,
                showBackingValue = showBackingValue,
                valueTypes       = types == null ? new List<Type>() : new List<Type>(types),
            };
            
            port.SetPortData(portData);
            
            node.AddPortValue(port);

            return port.Value;
        }

    }
}