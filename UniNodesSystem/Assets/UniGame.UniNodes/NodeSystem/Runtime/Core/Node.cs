namespace UniGreenModules.UniNodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using Interfaces;
    using UniCore.Runtime.Attributes;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Attributes;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core;
    using UnityEngine;

    [Serializable]
    public abstract class Node : MonoBehaviour, INode
    {

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
      
        /// <summary> Parent <see cref="NodeGraph"/> </summary>
        [Tooltip("Parent Graph")] 
        [HideNodeInspector] 
        [SerializeField] public NodeGraph graph;

        #region public properties

        public int Id => id == 0 ? UpdateId() : id;

        public string ItemName => nodeName;


        /// <summary> Iterate over all ports on this node. </summary>
        public IReadOnlyList<NodePort> Ports => ports.Ports;

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

        public virtual NodeGraph Graph => graph;

        #endregion
        
        public void OnIdUpdate(int oldId, int newId, IGraphItem updatedItem)
        {
            return;
        }
        
        public int UpdateId()
        {
            id = Graph.UpdateId(id);
            return id;
        }

        public virtual string GetName() => nodeName;

        public void SetGraph(NodeGraph parent)
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

            var port = new NodePort(this,fieldName, direction, connectionType,showBackingValue,types);
            port.Initialize(this);
            
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
            ports.Remove(port.ItemName);
        }

        /// <summary> Returns output port which matches fieldName </summary>
        public NodePort GetOutputPort(string fieldName)
        {
            var port = GetPort(fieldName);
            if (port == null || port.Direction != PortIO.Output) return null;
            return port;
        }

        /// <summary> Returns input port which matches fieldName </summary>
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
        
        /// <summary> Returns port which matches fieldName </summary>
        public NodePort GetPort(string portName)
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
    }
}