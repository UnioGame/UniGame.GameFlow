namespace UniGreenModules.UniNodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using UniCore.Runtime.Attributes;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
    using UnityEngine;
    using UnityEngine.Serialization;

    [Serializable]
    public abstract class UniBaseNode : MonoBehaviour, INode
    {

        [HideInInspector] 
        [ReadOnlyValue] 
        [SerializeField] 
        private ulong _id;

        [HideInInspector]
        [SerializeField]
        public int width = 220;

        [HideInInspector]
        [SerializeField] 
        public string nodeName;
        
        /// <summary> Position on the <see cref="NodeGraph"/> </summary>
        [SerializeField] public Vector2 position;

        /// <summary> It is recommended not to modify these at hand. Instead, see <see cref="NodeInputAttribute"/> and <see cref="NodeOutputAttribute"/> </summary>
        [SerializeField] private NodePortDictionary ports = new NodePortDictionary();
      
        /// <summary> Parent <see cref="NodeGraph"/> </summary>
        [SerializeField] 
        [Tooltip("Parent Graph")] 
        [HideInInspector] 
        [FormerlySerializedAs("graph")]
        private NodeGraph _graph;

        #region public properties
        
        public ulong Id
        {
            get
            {
                if (_id == 0)
                {
                    UpdateId();
                }

                return _id;
            }
        }

        public string ItemName => nodeName;
        
        
        /// <summary> Iterate over all ports on this node. </summary>
        public IReadOnlyList<NodePort> Ports => ports.Ports;

        
        #endregion
        
        public void UpdateId()
        {
            _id = Graph.GetId();
            foreach (var portPair in ports)
            {
                var port = portPair.Value;
                port.UpdateId();
            }
        }

        public virtual string GetName()
        {
            return nodeName;
        }

        /// <summary> Iterate over all outputs on this node. </summary>
        public IEnumerable<NodePort> Outputs
        {
            get
            {
                foreach (var port in Ports)
                {
                    if (port.IsOutput) yield return port;
                }
            }
        }

        /// <summary> Iterate over all inputs on this node. </summary>
        public IEnumerable<NodePort> Inputs
        {
            get
            {
                foreach (var port in Ports)
                {
                    if (port.IsInput) yield return port;
                }
            }
        }
        
        public NodeGraph Graph
        {
            get => _graph;
            set
            {
                if (_graph == value)
                    return;
                _graph = value;
                _id = _graph.GetId();
            }
        }

        // protected void OnEnable()
        // {
        //     UpdateStaticPorts();
        // }
        // /// <summary> Update static ports to reflect class fields. This happens automatically on enable. </summary>
        // public void UpdateStaticPorts()
        // {
        //     NodeDataCache.UpdatePorts(this, ports);
        // }

        /// <summary> Checks all connections for invalid references, and removes them. </summary>
        public void VerifyConnections()
        {
            foreach (var port in Ports) port.VerifyConnections();
        }

        #region Instance Ports

        /// <summary> Add a serialized port to this node. </summary>
        public INodePort AddPort(
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

            var port = new NodePort(this,fieldName, direction, connectionType,showBackingValue,types);
            ports.Add(fieldName, port);
            return port;
        }

        /// <summary> Remove an instance port from the node </summary>
        public void RemovePort(string fieldName)
        {
            RemovePort(GetPort(fieldName));
        }

        /// <summary> Remove an instance port from the node </summary>
        public virtual void RemovePort(NodePort port)
        {
            if (port == null) throw new ArgumentNullException("port");
            port.ClearConnections();
            ports.Remove(port.FieldName);
        }

        #endregion

        #region Ports

        /// <summary> Returns output port which matches fieldName </summary>
        public NodePort GetOutputPort(string fieldName)
        {
            var port = GetPort(fieldName);
            if (port == null || port.Direction != PortIO.Output) return null;
            else return port;
        }

        /// <summary> Returns input port which matches fieldName </summary>
        public NodePort GetInputPort(string fieldName)
        {
            var port = GetPort(fieldName);
            if (port == null || port.Direction != PortIO.Input) return null;
            else return port;
        }

        /// <summary> Returns port which matches fieldName </summary>
        public NodePort GetPort(string fieldName)
        {
            NodePort port;
            if (ports.TryGetValue(fieldName, out port)) return port;
            else return null;
        }

        public bool HasPort(string fieldName)
        {
            return ports.ContainsKey(fieldName);
        }

        #endregion
        
        /// <summary> Called after a connection between two <see cref="NodePort"/>s is created </summary>
        /// <param name="from">Output</param> <param name="to">Input</param>
        public virtual void OnCreateConnection(NodePort from, NodePort to)
        {
        }

        /// <summary> Called after a connection is removed from this port </summary>
        /// <param name="port">Output or Input</param>
        public virtual void OnRemoveConnection(NodePort port)
        {
        }

        /// <summary> Disconnect everything from this node </summary>
        public void ClearConnections()
        {
            foreach (var port in Ports) port.ClearConnections();
        }

        public override int GetHashCode()
        {
            if (Application.isPlaying)
            {
                var id = base.GetHashCode();
                return id;
            }

            return JsonUtility.ToJson(this).GetHashCode();
        }

    }
}