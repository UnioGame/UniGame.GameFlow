using UnityEngine;

namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Nodes;
    using Runtime.Interfaces;
    using UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.Runtime.Attributes;
    using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime;
    using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniGreenModules.UniGame.Core.Runtime.Attributes.FieldTypeDrawer;

    [Serializable]
    public class SerializableNode : INode
    {
        public static INode DummyNode = new DummyNode();
        
        #region inspector

        [HideInInspector]
        [HideNodeInspector] 
        [ReadOnlyValue] 
        [SerializeField] public int id;

        [HideInInspector]
        [HideNodeInspector]
        [SerializeField] public int width = 220;

        [HideInInspector]
        [HideNodeInspector]
        [SerializeField] public string nodeName;
        
        /// <summary> Position on the <see cref="NodeGraph"/> </summary>
        [HideInInspector]
        [SerializeField] public Vector2 position;

        /// <summary>
        /// It is recommended not to modify these at hand. Instead,
        /// see <see cref="NodeInputAttribute"/> and <see cref="NodeOutputAttribute"/>
        /// </summary>
        [HideInInspector]
        [SerializeField] public NodePortDictionary ports = new NodePortDictionary();
      
        #endregion
        
        protected IGraphData graph;
        
        private HashSet<INodePort> portValues;
        
        #region constructor

        public SerializableNode(){}
        
        public SerializableNode(
            int id,
            string name,
            NodePortDictionary ports)
        {
            this.id       = id;
            this.nodeName = name;
            this.ports    = ports;
        }
        
        #endregion
        
        #region public properties

        public HashSet<INodePort> RuntimePorts => portValues = 
            portValues ?? (portValues = new HashSet<INodePort>());
        
        /// <summary>
        /// unique node id
        /// </summary>
        public int Id => id == 0 ? UpdateId() : id;

        /// <summary>
        /// Node name
        /// </summary>
        public string ItemName => nodeName;

        /// <summary>
        /// Iterate over all ports on this node.
        /// </summary>
        public IEnumerable<INodePort> Ports => ports.Values;

        /// <summary>
        /// node width
        /// </summary>
        public int Width {
            get => width;
            set => width = value;
        }
        
        /// <summary>
        /// position of node 
        /// </summary>
        public Vector2 Position {
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
        public virtual IGraphData GraphData => graph;

        #endregion
        
        #region abstract methods

        public int SetId(int itemId)
        {
            id = itemId;
            return id;
        }

        public virtual void Initialize(IGraphData data)
        {
            graph = data;
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

        public void SetUpData(IGraphData parent)
        {
            if (graph == parent)
                return;
            graph = parent;
            UpdateId();       
        }
        
        /// <summary> Add a serialized port to this node. </summary>
        public NodePort AddPort(
            string fieldName,
            IReadOnlyList<Type> types, PortIO direction,
            ConnectionType connectionType = ConnectionType.Multiple,
            ShowBackingValue showBackingValue = ShowBackingValue.Always)
        {
            var port = HasPort(fieldName) ? 
                ports[fieldName] :
                new NodePort(GraphData.GetId(),this, fieldName, direction, connectionType,showBackingValue,types);
            return AddPort(port);
        }

        public NodePort AddPort(NodePort port)
        {
            var portName = port.ItemName;

            AddPortValue(port);
            
            if (!HasPort(portName))
            {
                ports.Add(portName,port);
            }    
            
            port.Initialize(this);
            
            return port;
        }

        public void SetPosition(Vector2 newPosition)
        {
            position = newPosition;
        }

        /// <summary>
        /// Remove an instance port from the node
        /// </summary>
        public void RemovePort(string fieldName)
        {
            RemovePort(GetPort(fieldName));
        }

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
            if (port == null || port.Direction != PortIO.Output) return null;
            return port;
        }

        /// <summary>
        /// Returns input port which matches fieldName
        /// </summary>
        public INodePort GetInputPort(string fieldName)
        {
            var port = GetPort(fieldName);
            if (port == null || port.Direction != PortIO.Input) return null;
            return port;
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
            foreach (var portPair in ports) {
                var port = portPair.Value;
                if (port == null || string.IsNullOrEmpty(port.fieldName)) {
                    removedPorts.Add(port);
                    continue;
                }

                var value = RuntimePorts.FirstOrDefault(x => x.ItemName == port.ItemName &&
                                                            x.Direction == port.Direction);
                if (value == null || string.IsNullOrEmpty(value.ItemName)) {
                    removedPorts.Add(port);
                }
            }

            removedPorts.ForEach(RemovePort);
            removedPorts.Despawn();
        }

        #endregion

        protected bool AddPortValue(INodePort runtimePort)
        {
            portValues = portValues ?? new HashSet<INodePort>();  
            
            if (runtimePort == null) {
                GameLog.LogErrorFormat("Try add NULL port value to {0}", this);
                return false;
            }

            portValues.Add(runtimePort);

            return true;
        }

        protected IEnumerable<INodePort> GetPorts(PortIO direction)
        {
            foreach (var port in Ports) {
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
