namespace UniGreenModules.UniNodeSystem.Runtime.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Interfaces;
    using UniCore.Runtime.ObjectPool.Runtime;
    using UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniCore.Runtime.ProfilerTools;
    using UniCore.Runtime.Rx.Extensions;
    using UniCore.Runtime.Utils;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
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
        
        public static List<TTarget> GetOutputConnections<TTarget>(this INode node)
            where TTarget :UniBaseNode
        {
            var items = ClassPool.Spawn<List<TTarget>>();
            
            foreach (var nodeOutput in node.Outputs)
            {
                var connectedNode = nodeOutput.GetConnectedNode<TTarget>();
                if(connectedNode == null)
                    continue;
                items.Add(connectedNode);
            }

            return items;
        }

            
        public static (IPortValue inputValue, IPortValue outputValue) 
            CreatePortPair(this IUniNode node, string outputPortName, bool connectInOut = false)
        {
            var inputName = outputPortName.GetFormatedPortName(PortIO.Input);
            return node.CreatePortPair(inputName, outputPortName, connectInOut);
        }

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
        
        public static List<INodePort> GetConnectionToNodes<TTarget>(this INodePort port)
            where TTarget :UniBaseNode
        {
            
            var items = ClassPool.Spawn<List<INodePort>>();
            
            var connections = port.GetConnections();
            
            foreach (var connection in connections)
            {
                if(!(connection.Node is TTarget node))
                    continue;
                
                items.Add(connection);
            }
            
            connections.DespawnCollection();
            
            return items;
            
        }
        
        public static List<TTarget> GetConnectedNodes<TTarget>(this INodePort port)
            where TTarget :IUniNode
        {
            var items = ClassPool.Spawn<List<TTarget>>();
            
            var connections = port.GetConnections();
            
            foreach (var connection in connections)
            {
                if(!(connection.Node is TTarget node))
                    continue;
                items.Add(node);
            }
            
            connections.DespawnCollection();
            
            return items;
        }

        public static TValue GetConnectedNode<TValue>(this INodePort port)
            where TValue :UniBaseNode
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
            
            return node.UpdatePortValue(
                portData.FieldName, 
                portData.Direction, 
                portData.ConnectionType,
                ShowBackingValue.Always, 
                portData.ValueTypes);
        }

        public static IPortValue UpdatePortValue(
            this IUniNode node,
            string portName,
            PortIO direction = PortIO.Output,
            ConnectionType connectionType = ConnectionType.Multiple, 
            ShowBackingValue showBackingValue = ShowBackingValue.Always,
            IReadOnlyList<Type> types = null)
        {
            INodePort port = node.GetPort(portName);
            
            if (port == null) {
                types = types ?? new List<Type>();
                port = node.AddPort(portName, types, direction, connectionType, showBackingValue);
            }

            node.AddPortValue(port);

            return port.Value;
        }

    }
}