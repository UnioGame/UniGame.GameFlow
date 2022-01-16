using UniCore.Runtime.ProfilerTools;
using UnityEngine;

namespace UniModules.GameFlow.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Nodes;
    using Runtime.Interfaces;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.Attributes;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniModules.UniGame.Core.Runtime.Attributes;
    using UniModules.UniGame.Core.Runtime.Attributes.FieldTypeDrawer;
    using UniModules.UniGame.Core.Runtime.Interfaces;

    [Serializable]
    public class SerializableNode : INode
    {
        public static INode DummyNode = new DummyNode();

        #region inspector

        [HideInInspector]
        [ReadOnlyValue] 
        [SerializeField] public int id;

        [HideInInspector]
        [HideNodeInspector]
        [SerializeField]
        public int width = 220;

        [HideInInspector]
        [HideNodeInspector]
        [SerializeField]
        public string nodeName;

        /// <summary> Position on the <see cref="NodeGraph"/> </summary>
        [HideInInspector]
        [SerializeField]
        public Vector2 position;

        /// <summary>
        /// It is recommended not to modify these at hand. Instead,
        /// see <see cref="NodeInputAttribute"/> and <see cref="NodeOutputAttribute"/>
        /// </summary>
        [SerializeField]
        [IgnoreDrawer]
        public NodePortDictionary ports = new NodePortDictionary();

        [HideInInspector]
        public NodeGraph graphData;
        
        #endregion

        private HashSet<INodePort> _portValues;

        #region public properties

        public HashSet<INodePort> RuntimePorts => _portValues ??= new HashSet<INodePort>();

        public virtual IContext Context => GraphData.GraphContext;

        /// <summary>
        /// unique node id
        /// </summary>
        public int Id => id;

        /// <summary>
        /// Node name
        /// </summary>
        public virtual string ItemName => nodeName;

        /// <summary>
        /// Iterate over all ports on this node.
        /// </summary>
        public IEnumerable<INodePort> Ports => ports.Values;

        /// <summary>
        /// node width
        /// </summary>
        public int Width
        {
            get => width;
            set => width = value;
        }

        /// <summary>
        /// position of node 
        /// </summary>
        public Vector2 Position
        {
            get => position;
            set => position = value;
        }

        /// <summary>
        /// Iterate over all outputs on this node.
        /// </summary>
        public IEnumerable<INodePort> Outputs => GetPorts(PortIO.Output);

        /// <summary>
        /// Iterate over all inputs on this node.
        /// </summary>
        public IEnumerable<INodePort> Inputs => GetPorts(PortIO.Input);

        /// <summary>
        /// base context graph data
        /// </summary>
        public virtual NodeGraph GraphData
        {
            get => graphData;
            protected set => graphData = value;
        }

        #endregion

        #region abstract methods

        public int SetId(int itemId)
        {
            id = itemId;
            return id;
        }

        public virtual void Initialize(NodeGraph data)
        {
            graphData = data;
            RuntimePorts.Clear();
        }

        #endregion

        #region public methods

        public int UpdateId()
        {
            id = GraphData.UpdateId(id);
            return id;
        }

        public virtual string GetName() => nodeName;

        public void SetUpData(NodeGraph parent) => graphData = parent;

        /// <summary> Add a serialized port to this node. </summary>
        public NodePort AddPort(
            string fieldName,
            IEnumerable<Type> types, 
            PortIO direction,
            ConnectionType connectionType = ConnectionType.Multiple,
            ShowBackingValue showBackingValue = ShowBackingValue.Always,
            bool distinctValue = false)
        {
            var port = HasPort(fieldName) 
                ? ports[fieldName] 
                : new NodePort(GraphData.GetNextId(), Id,graphData, fieldName, direction, connectionType, showBackingValue, types,distinctValue);
            
            return AddPort(port);
        }

        public NodePort AddPort(NodePort port)
        {
            var portName = port.ItemName;

            AddPortValue(port);

            if (!HasPort(portName))
                ports.Add(portName, port);
            
            port.Initialize(Id,graphData);

            return port;
        }

        public void SetPosition(Vector2 newPosition) => position = newPosition;

        /// <summary>
        /// Remove an instance port from the node
        /// </summary>
        public void RemovePort(string fieldName) => RemovePort(GetPort(fieldName));

        /// <summary>
        /// Remove an instance port from the node
        /// </summary>
        public virtual void RemovePort(INodePort port)
        {
            if (port == null) throw new ArgumentNullException("port");
            port.ClearConnections();
            ports.Remove(port.ItemName);
        }

        /// <summary>
        /// Returns output port which matches fieldName
        /// </summary>
        public INodePort GetOutputPort(string fieldName)
        {
            var port = GetPort(fieldName);
            return !(port is { Direction: PortIO.Output }) 
                ? null : port;
        }

        /// <summary>
        /// Returns input port which matches fieldName
        /// </summary>
        public INodePort GetInputPort(string fieldName)
        {
            var port = GetPort(fieldName);
            return !(port is { Direction: PortIO.Input }) 
                ? null : port;
        }

        public IPortValue GetPortValue(string portName)
        {
            var port = GetPort(portName);
            return port?.Value;
        }

        /// <summary>
        /// Returns port which matches fieldName
        /// </summary>
        public INodePort GetPort(string portName)
        {
            if (string.IsNullOrEmpty(portName))
                return null;

            return ports.TryGetValue(portName, out var port) ? port : null;
        }

        public bool HasPort(string fieldName)
        {
            return ports.ContainsKey(fieldName);
        }

        /// <summary> Disconnect everything from this node </summary>
        public void ClearConnections()
        {
            foreach (var port in Ports) port.ClearConnections();
        }

        public void SetName(string itemName)
        {
            nodeName = itemName;
        }

        public virtual void Validate()
        {
            var removedPorts = ClassPool.Spawn<List<NodePort>>();

            foreach (var portPair in ports)
            {
                var port = portPair.Value;
                if (port == null || string.IsNullOrEmpty(port.fieldName))
                {
                    removedPorts.Add(port);
                    continue;
                }

                var value = RuntimePorts.FirstOrDefault(x => x.ItemName == port.ItemName &&
                                                             x.Direction == port.Direction);
                if (value == null || string.IsNullOrEmpty(value.ItemName))
                {
                    removedPorts.Add(port);
                    continue;
                }

                port.Validate();
            }

            removedPorts.ForEach(RemovePort);
            removedPorts.Despawn();
        }

        public virtual string GetStyle() => string.Empty;

        #endregion

        protected bool AddPortValue(INodePort runtimePort)
        {
            _portValues ??= new HashSet<INodePort>();

            if (runtimePort == null)
            {
                GameLog.LogErrorFormat("Try add NULL port value to {0}", this);
                return false;
            }

            _portValues.Add(runtimePort);

            return true;
        }

        protected IEnumerable<INodePort> GetPorts(PortIO direction)
        {
            foreach (var port in Ports)
            {
                if (port.Direction == direction)
                    yield return port;
            }
        }

        [Conditional("UNITY_EDITOR")]
        protected void LogMessage(string message)
        {
            GameLog.Log($"{GraphData.ItemName}:{ItemName}: {message}");
        }
    }
}