namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Runtime.Extensions;
    using Runtime.Interfaces;
    using UniModules.UniContextData.Runtime.Entities;
    using UniModules.UniCore.Runtime.Attributes;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniRx;
    using UnityEngine;
    using Object = UnityEngine.Object;

    /// <summary> Base class for all node graphs </summary>
    [Serializable]
    public abstract class NodeGraph : UniNode, INodeGraph
    {
        #region static data

        public static ReactiveCollection<NodeGraph> ActiveGraphs { get; } = new ReactiveCollection<NodeGraph>();

        public static Type objectType = typeof(Object);
        
        #endregion
        
        #region inspector data
        
        [ReadOnlyValue] 
        [SerializeField] private int uniqueId;

        [SerializeField]
        public List<Node> nodes = new List<Node>();
  
        [SerializeReference]
        public List<INode> serializableNodes = new List<INode>();

        [HideInInspector]
        [SerializeReference]
        public List<INodesGroup> nodeGroups = new List<INodesGroup>();

        #endregion

        private EntityContext _graphContext = new EntityContext();
        
        private List<INode> _allNodes = new List<INode>();
        
        [NonSerialized] 
        private Dictionary<int, INode> nodesCache;

        #region public properties

        public IContext Context => _graphContext;
        
        public IReadOnlyList<INode> Nodes => GetNodes();

        public IReadOnlyList<INode> SerializableNodes => serializableNodes;
        
        public IReadOnlyList<INode> ObjectNodes => nodes;

        public sealed override IGraphData GraphData => this;

        #endregion

        public void Dispose() => Exit();

        #region graph operations

        protected override void OnExecute() => LifeTime.AddCleanUpAction(() => _graphContext.Release());

        /// <summary>
        /// get unique Id in graph scope
        /// </summary>
        /// <returns></returns>
        public int GetId() => ++uniqueId;

        public int UpdateId(int oldId) => GetId();

        public IReadOnlyList<INode> GetNodes()
        {
            _allNodes = _allNodes ?? new List<INode>();
            if (_allNodes.Count > 0)
                return _allNodes;
            
            _allNodes.AddRange(nodes);
            _allNodes.AddRange(serializableNodes);
            return _allNodes;
        }
        
        /// <summary>
        /// Add a node to the graph by type
        /// </summary>
        public T AddNode<T>() where T : class, INode => AddNode(typeof(T)) as T;

        /// <summary>
        /// Add a node to the graph by type
        /// </summary>
        public T AddNode<T>(string name) where T : class, INode => AddNode(name, typeof(T)) as T;

        /// <summary>
        /// return node by it ID
        /// </summary>
        public INode GetNode(int nodeId)
        {
            nodesCache = nodesCache ?? new Dictionary<int, INode>();
            if (nodesCache.Count != nodes.Count) {
                nodesCache.Clear();
                nodesCache = Nodes.ToDictionary(x => x.Id);
            }

            nodesCache.TryGetValue(nodeId, out var node);
            return node;
        }

        /// <summary>
        /// add node by type
        /// </summary>
        public virtual INode AddNode(string itemName, Type type)
        {
            var node = objectType.IsAssignableFrom(type) ? 
                AddAssetNode(type) : 
                AddSerializableNode(type);

            if (node == null) return null;
            
            node.SetUpData(this);
            node.Initialize(this);
            node.SetName(itemName);
            node.UpdateNodePorts();
            
            return node;
        }

        /// <summary>
        /// Add a node to the graph by type
        /// </summary>
        public INode AddNode(Type type) => AddNode(type.Name, type);

        /// <summary>
        /// add node into the graph with position and name
        /// </summary>
        public INode AddNode(Type type,string itemName, Vector2 nodePosition)
        {
            var node = AddNode(itemName, type);
            if(node!=null) 
                node.Position = (nodePosition);
            return node;
        } 

        /// <summary>
        /// Creates a copy of the original node in the graph
        /// </summary>
        public virtual Node CopyNode(Node original)
        {
            var node = Instantiate(original);
            node.SetId(GetId());
            node.SetUpData(this);
            node.ClearConnections();
            nodes.Add(node);
            return node;
        }

        /// <summary>
        /// Safely remove a node and all its connections
        /// </summary>
        public IGraphData RemoveNode(INode node)
        {
            node.ClearConnections();

            var nodeAsset = node as Node;
            if (nodeAsset == null) {
                serializableNodes.Remove(node);
                return this;
            }
            
            nodes.Remove(nodeAsset);

            if (Application.isPlaying) {
                Destroy(nodeAsset);
            }
            else if (Application.isEditor) {
                DestroyImmediate(nodeAsset,true);
            }

            return this;
        }

        [ContextMenu("Validate")]
        public override void Validate()
        {
            graph = this;
            nodes.Clear();
            _allNodes?.Clear();
            
            serializableNodes.RemoveAll(x => x == null || x is Object);
            
            nodes.AddRange(GetComponents<Node>());
            nodes.RemoveAll(x => !x);
            nodes.Remove(this);

            var nodeItems = ClassPool.Spawn<List<INode>>();
            nodeItems.AddRange(serializableNodes);
            nodeItems.AddRange(nodes);

            foreach (var nodeItem in nodeItems) {
                nodeItem.SetUpData(this);
                nodeItem.Validate();
            }
            
            nodeItems.Despawn();
        }

        #endregion
        
        #region private methods

        protected override void OnInitialize() => _allNodes?.Clear();

        private INode AddAssetNode(Type type)
        {
            var component = gameObject.AddComponent(type);
            var node      = component as INode;
            
            switch (node) {
                case null:
                    DestroyImmediate(component, true);
                    return null;
                case Node nodeAsset:
                    nodes.Add(nodeAsset);
                    break;
            }

            return node;
        }
        
        private INode AddSerializableNode(Type type)
        {
            var node = Activator.CreateInstance(type) as INode;
            if (node != null) {
                serializableNodes.Add(node);
            }
            return node;
        }
        
        #endregion

    }
}