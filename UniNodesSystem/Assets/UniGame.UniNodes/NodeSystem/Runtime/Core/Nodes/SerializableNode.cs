using UnityEngine;

namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Attributes;
    using Interfaces;
    using Nodes;
    using Runtime.Interfaces;
    using UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.Runtime.Attributes;
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
        
        protected HashSet<INodePort> portValues;
        
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

        public IReadOnlyCollection<INodePort> PortValues => portValues;

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
        public IReadOnlyList<INodePort> Ports => ports.Ports;

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
            var oldId = id;
            var newId = itemId;
            id = itemId;
            OnIdUpdate(oldId,newId,this);
            return id;
        }

        public virtual void Initialize(IGraphData data)
        {
            graph = data;
        }

        #endregion
        
        #region public methods
        
        public void OnIdUpdate(int oldId, int newId, IGraphItem updatedItem)
        {
            return;
        }
        
        public int UpdateId()
        {
            id = GraphData.UpdateId(id);
            return id;
        }

        public bool AddPortValue(INodePort portValue)
        {
            if (portValue == null) {
                GameLog.LogErrorFormat("Try add NULL port value to {0}", this);
                return false;
            }

            portValues.Add(portValue);

            return true;
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
            if (HasPort(fieldName))
            {
                GameLog.LogWarning("Port '" + fieldName + "' already exists in " + ItemName);
                return ports[fieldName];
            }    

            var port = new NodePort(GraphData.GetId(),this, fieldName, direction, connectionType,showBackingValue,types);
            port.Initialize(this);
            
            ports.Add(fieldName, port);
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

        public virtual void Validate(){}

        public void SetName(string itemName)
        {
            nodeName = itemName;
        }

        #endregion

        protected IEnumerable<INodePort> GetPorts(PortIO direction)
        {
            for (var i = 0; i < Ports.Count; i++) {
                var port = Ports[i];
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
