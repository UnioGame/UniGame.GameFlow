namespace UniModules.GameFlow.Runtime.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Core;
    using Core.Interfaces;
    using Interfaces;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniCore.Runtime.Utils;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Extensions;
    using UniRx;
    using UnityEngine;

    
    public static class UniNodeExtension
    {
        public const int InputNameIndex  = 0;
        public const  int OutputNameIndex = 1;

        public const string InputPattern = @"(\[?[\w\num ]*\])";
        
        public const string InputTriggerPrefix  = "[in]";
        public const string OutputTriggerPrefix = "[out]";       
        
        public static Func<string, string[]> portNameCache = MemorizeTool.Create((string x) => new string[2]);

#region port names

        [Conditional("UNITY_EDITOR")]
        public static void UpdatePorts(this INode node, IGraphData data)
        {
            var portList = ClassPool.Spawn<List<INodePort>>();

            node.Initialize(data);
            
            portList.AddRange(node.Ports);

            node.UpdateNodePorts();

            portList.Despawn();
        }
        

        [Conditional("UNITY_EDITOR")]
        public static void UpdateNodePorts(this INode node)
        {
            if (Application.isPlaying)
                return;
            
            UniGraphEvent.NodeUpdateStream.OnNext(node);
        }
        
        public static void UpdateSerializedCommands(INode node,IPortValue port, object value)
        {

            switch (value) {
                case IReactiveSource reactiveSource:
                    reactiveSource.Bind(node,port.ItemName);
                    return;
            }

        }
        
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
                inputValue.Broadcast(outputValue);
        
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

        public static INodePort UpdatePort(this INode node, IPortData portData)
        {
            var portValue = UpdatePortValue(node, portData);
            if (portValue == null)
                return null;
            var port = node.GetPort(portValue.ItemName);
            return port;
        }

        public static IPortValue UpdatePortValue(this INode node , IPortData portData)
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
            this INode node,
            string portName,
            PortIO direction = PortIO.Output,
            ConnectionType connectionType = ConnectionType.Multiple, 
            ShowBackingValue showBackingValue = ShowBackingValue.Always,
            IReadOnlyList<Type> types = null)
        {
            types = types ?? new List<Type>();
            var port = node.AddPort(portName, types, direction, connectionType, showBackingValue);
            
            return port.Value;
        }

    }
}