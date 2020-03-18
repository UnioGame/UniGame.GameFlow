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
    using UnityEngine;
    using Debug = UnityEngine.Debug;

    [Serializable]
    public abstract class Node : MonoBehaviour, INode
    {
        public static INode DummyNode = new DummyNode();
        
        #region inspector

        [HideNodeInspector] 
        [ReadOnlyValue] 
        [SerializeField] public int id;

        [HideNodeInspector]
        [SerializeField] public int width = 220;

        [HideNodeInspector]
        [SerializeField] public string nodeName;
        
        /// <summary> Position on the <see cref="NodeGraph"/> </summary>
        [SerializeField] public Vector2 position;

        /// <summary> It is recommended not to modify these at hand. Instead, see <see cref="NodeInputAttribute"/> and <see cref="NodeOutputAttribute"/> </summary>
        [SerializeField] public NodePortDictionary ports = new NodePortDictionary();
      
        #endregion

        protected IGraphData graph;

        #region public properties

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
        public IReadOnlyList<NodePort> Ports => ports.Ports;
        
        /// <summary>
        /// Iterate over all outputs on this node.
        /// </summary>
        public IEnumerable<NodePort> Outputs
        {
            get {
                for (var i = 0; i < Ports.Count; i++) {
                    var port = Ports[i];
                    if (port.IsOutput) yield return port;
                }
            }
        }

        /// <summary>
        /// Iterate over all inputs on this node.
        /// </summary>
        public IEnumerable<NodePort> Inputs
        {
            get {
                for (var i = 0; i < Ports.Count; i++) {
                    var port = Ports[i];
                    if (port.IsInput) yield return port;
                }
            }
        }

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
        /// base context graph data
        /// </summary>
        public virtual IGraphData GraphData => graph;

        #endregion
        
        #region abstract methods

        public abstract void Initialize(IGraphData data);

        #endregion
        
        #region public methods

        public virtual void OnIdUpdate(int oldId, int newId, IGraphItem updatedItem)
        {
        
        }
        
        public int UpdateId()
        {
            var oldId = id;
            id = GraphData.UpdateId(id);
            OnIdUpdate(oldId,id,this);
            return id;
        }

        public int SetId(int itemId)
        {
            var oldId = id;
            id = itemId;
            OnIdUpdate(oldId,id,this);
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
            if (fieldName == null)
            {
                fieldName = "instanceInput_0";
                var i = 0;
                while (HasPort(fieldName)) fieldName = "instanceInput_" + (++i);
            }
            else if (HasPort(fieldName))
            {
                Debug.LogWarning("Port '" + fieldName + "' already exists in " + name, this);
                return ports[fieldName];
            }    

            var port = new NodePort(GraphData.GetId(),this,fieldName, direction, connectionType,showBackingValue,types);
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
        public virtual void RemovePort(NodePort port)
        {
            if (port == null) throw new ArgumentNullException("port");
            port.ClearConnections();
            ports.Remove(port.ItemName);
        }

        /// <summary>
        /// Returns output port which matches fieldName
        /// </summary>
        public NodePort GetOutputPort(string fieldName)
        {
            var port = GetPort(fieldName);
            if (port == null || port.Direction != PortIO.Output) return null;
            return port;
        }

        /// <summary>
        /// Returns input port which matches fieldName
        /// </summary>
        public NodePort GetInputPort(string fieldName)
        {
            var port = GetPort(fieldName);
            if (port == null || port.Direction != PortIO.Input) return null;
            return port;
        }

        public IPortValue GetPortValue(string portName)
        {
            var port = GetPort(portName);
            return port?.portValue;
        }
        
        /// <summary>
        /// Returns port which matches fieldName
        /// </summary>
        public NodePort GetPort(string portName)
        {
            if (string.IsNullOrEmpty(portName))
                return null;

            return ports.TryGetValue(portName, out var port) ? port : null;
        }

        public void SetWidth(int nodeWidth)
        {
            width = nodeWidth;
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

        [Conditional("UNITY_EDITOR")]
        protected void LogMessage(string message)
        {
            GameLog.Log($"{GraphData.ItemName}:{ItemName}: {message}");
        }
    }
}