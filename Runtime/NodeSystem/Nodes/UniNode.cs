using System.Diagnostics;
using UniCore.Runtime.ProfilerTools;
using UniModules.GameFlow.Runtime.Core.Interfaces;
using UniModules.GameFlow.Runtime.Core.Nodes;
using UniModules.UniCore.Runtime.Attributes;
using UniModules.UniGame.Context.Runtime.Connections;
using UniModules.UniGame.Core.Runtime.Attributes;
using UniModules.UniGame.Core.Runtime.Attributes.FieldTypeDrawer;
using UnityEngine;

namespace UniModules.GameFlow.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using Attributes;
    using Cysharp.Threading.Tasks;
    using Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    
#if UNITY_EDITOR
    using UnityEditor;
#endif
    
    [HideNode]
    [Serializable]
    public abstract class UniNode : MonoBehaviour, IUniNode
    {
        public static INode DummyNode = new DummyNode();
        
        #region inspector

        [HideNodeInspector] 
        [ReadOnlyValue] 
        [SerializeField]
        public int id;

        [HideInInspector]
        [HideNodeInspector]
        [SerializeField] public int width = 250;

        [HideInInspector]
        [HideNodeInspector]
        [SerializeField] public string nodeName;
        
        /// <summary> Position on the <see cref="NodeGraph"/> </summary>
        [HideInInspector]
        [HideNodeInspector] 
        [SerializeField] 
        public Vector2 position;

        /// <summary> It is recommended not to modify these at hand. Instead, see <see cref="NodeInputAttribute"/> and <see cref="NodeOutputAttribute"/> </summary>
        [SerializeField] 
        [HideNodeInspector] 
        [IgnoreDrawer]
        [HideInInspector]
        public NodePortDictionary ports = new NodePortDictionary();

        [HideInInspector]
        public NodeGraph graph;
        
        #endregion
        
        private IProxyNode _serializableNode;

        #region public properties

        public virtual IContextConnection Context => SNode.Context;
        
        /// <summary>
        /// unique node id
        /// </summary>
        public int Id => id != 0 ? id : SetId(graph.GetNextId());

        /// <summary>
        /// Node name
        /// </summary>
        public virtual string ItemName => nodeName;

        /// <summary>
        /// Iterate over all ports on this node.
        /// </summary>
        public IEnumerable<INodePort> Ports => SNode.Ports;

        /// <summary>
        /// Iterate over all outputs on this node.
        /// </summary>
        public IEnumerable<INodePort> Outputs => SNode.Outputs;

        /// <summary>
        /// Iterate over all inputs on this node.
        /// </summary>
        public IEnumerable<INodePort> Inputs => SNode.Inputs;

        /// <summary>
        /// node width
        /// </summary>
        public int Width {
            get => width;
            set { 
                width = value;  
                SNode.Width = value;
            }
        }
        
        /// <summary>
        /// position of node 
        /// </summary>
        public Vector2 Position {
            get => position;
            set {
                position = value;
                SNode.Position = value;
            }
        }

        /// <summary>
        /// base context graph data
        /// </summary>
        public virtual NodeGraph GraphData => graph;

        /// <summary>
        /// Is node currently active
        /// </summary>
        public bool IsActive => SNode.IsActive;

        public ILifeTime LifeTime => SNode.LifeTime;

        #endregion
        
        /// <summary>
        /// regular source node
        /// </summary>
        protected IProxyNode SNode => GetSourceNode();

        #region public methods

        public void Initialize(NodeGraph graphData)
        {
            graph = graphData;
            if (id != 0) SNode.SetId(id);
            SNode.Initialize(GraphData,OnInitialize, UpdateCommands, OnExecute);
        }

         public virtual void OnIdUpdate(int oldId, int newId, IGraphItem updatedItem) { }

        public int SetId(int itemId)
        {
            id = itemId;
            return SNode.SetId(itemId);
        }

        public void SetUpData(NodeGraph parent)
        {
            if (graph == parent)
                return;
            graph = parent;
            SNode.SetUpData(parent);
        }
        
        /// <summary>
        /// Add a serialized port to this node.
        /// </summary>
        public NodePort AddPort(
            string fieldName,
            IEnumerable<Type> types, 
            PortIO direction,
            ConnectionType connectionType = ConnectionType.Multiple,
            ShowBackingValue showBackingValue = ShowBackingValue.Always,
            bool distinctValue = false)
        {
            return SNode.AddPort(fieldName, types, direction, connectionType, showBackingValue);
        }

        public void SetPosition(Vector2 newPosition) => SNode.Position = position;

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
            SNode.RemovePort(port);
        }

        /// <summary>
        /// Returns output port which matches fieldName
        /// </summary>
        public INodePort GetOutputPort(string fieldName) => SNode.GetOutputPort(fieldName);

        /// <summary>
        /// Returns input port which matches fieldName
        /// </summary>
        public INodePort GetInputPort(string fieldName) => SNode.GetInputPort(fieldName);

        /// <summary>
        /// Returns port which matches fieldName
        /// </summary>
        public INodePort GetPort(string portName) => SNode.GetPort(portName);

        public IPortValue GetPortValue(string portName) => SNode.GetPort(portName)?.Value;

        public bool HasPort(string fieldName) => SNode.HasPort(fieldName);

        /// <summary> Disconnect everything from this node </summary>
        public void ClearConnections()
        {
            foreach (var port in Ports) port.ClearConnections();
        }

        public void SetName(string itemName) => nodeName = itemName;

        public virtual string GetStyle() => string.Empty;

        /// <summary>
        /// stop execution
        /// </summary>
        public void Exit() => SNode.Exit();

        /// <summary>
        /// start node execution
        /// </summary>
        public async UniTask ExecuteAsync()
        {
            Initialize(GraphData);
            await SNode.ExecuteAsync().AttachExternalCancellation(LifeTime.TokenSource);
        }

        /// <summary>
        /// stop node execution
        /// </summary>
        public void Release() => SNode.Release();

        public virtual void Validate() => SNode.Validate();
        
        #endregion
        
        #region private methods

        /// <summary>
        /// Call once on node initialization
        /// </summary>
        protected virtual void OnInitialize(){}

        /// <summary>
        /// base logic realization
        /// </summary>
        protected virtual UniTask OnExecute() => UniTask.CompletedTask;

        /// <summary>
        /// update active list commands
        /// add all supported node commands here
        /// </summary>
        protected virtual void UpdateCommands(List<ILifeTimeCommand> nodeCommands){}

        /// <summary>
        /// create base node realization
        /// </summary>
        protected virtual IProxyNode CreateInnerNode() => new SNode()
        {
            id = id,
            nodeName = nodeName,
            ports = ports
        };

        [Conditional("UNITY_EDITOR")]
        protected void LogMessage(string message)
        {
#if UNITY_EDITOR
            GameLog.Log($"{GraphData.ItemName}:{ItemName}: {message}");
#endif
        }
        
        /// <summary>
        /// create target source node and bind with mono node methods
        /// </summary>
        /// <returns></returns>
        private IProxyNode GetSourceNode()
        {
            if (_serializableNode != null)
                return _serializableNode;
            _serializableNode = CreateInnerNode();
            return _serializableNode;
        }
        
        #endregion
        
    }
}