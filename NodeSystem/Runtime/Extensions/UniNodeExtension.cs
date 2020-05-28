namespace UniGame.UniNodes.NodeSystem.Runtime.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Core;
    using Core.Interfaces;
    using Inspector.Editor.UniGraphWindowInspector.BaseEditor.Extensions;
    using Interfaces;
    using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime;
    using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniCore.Runtime.Utils;
    using UniRx;
    using UnityEngine;

    public static class UniNodeExtension
    {
        private const int InputNameIndex = 0;
        private const int OutputNameIndex = 1;

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

            node.UpdatePortByAttributes();

            portList.Despawn();
        }
        
        [Conditional("UNITY_EDITOR")]
        public static void UpdatePortByAttributes(this INode node)
        {
            if (Application.isPlaying)
                return;
            
            var type = node.GetType();
            
            var fields = type.GetFields(
                BindingFlags.Public | 
                BindingFlags.Instance | 
                BindingFlags.NonPublic);

            foreach (var portField in fields) {
                var data = node.GetPortData(portField, portField.Name);
                if(data.PortData == null)
                    continue;
                        
                var port  = node.UpdatePortValue(data.PortData);
                var value = portField.GetValue(node);

                UpdateSerializedCommands(node, port, value);
            }

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