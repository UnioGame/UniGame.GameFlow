namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Runtime.Extensions;
    using Runtime.Interfaces;
    using UniModules.UniCore.Runtime.Attributes;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniModules.UniGame.Context.Runtime.Context;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UnityEngine;
    using Object = UnityEngine.Object;

    /// <summary> Base class for all node graphs </summary>
    [Serializable]
    public abstract class NodeGraph : UniNode, INodeGraph
    {
        #region static data

        public static Type objectType = typeof(Object);
        
        #endregion
        
        #region inspector data

        public bool exitOnDisable = true;

        public bool activateOnEnable = false;

        public string guid = System.Guid.NewGuid().ToString();
        
        [ReadOnlyValue] 
        [SerializeField] private int uniqueId;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineEditor(Expanded = false)]
#endif
        [SerializeField]
        public List<Node> nodes = new List<Node>();
  
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineProperty]
#endif
        [SerializeReference]
        public List<INode> serializableNodes = new List<INode>();

        [HideInInspector]
        [SerializeReference]
        public List<INodesGroup> nodeGroups = new List<INodesGroup>();

        #endregion

        private EntityContext _graphContext = new EntityContext();
        
        private List<INode> _allNodes = new List<INode>();
        
        [NonSerialized] 
        private Dictionary<int, INode> _nodesCache;

        #region public properties

        public override string ItemName => name;

        public IContext Context => _graphContext;
        
        public IReadOnlyList<INode> Nodes => GetNodes();

        public IReadOnlyList<INode> SerializableNodes => serializableNodes;
        
        public IReadOnlyList<INode> ObjectNodes => nodes;

        public sealed override IGraphData GraphData => this;

        public string Guid => guid;
        
        #endregion

        public void Dispose() => Exit();

        #region graph operations

        protected override void OnExecute() => LifeTime.AddCleanUpAction(() => _graphContext.Release());

        /// <summary>
        /// get unique Id in graph scope
        /// </summary>
        /// <returns></returns>
        public int GetId()
        {
            var originId = GetIdFromOriginSource();
            var maxId = originId.id;
            maxId           = Mathf.Max(maxId,nodes.Count <= 0 ? 0 : nodes.Max(x => x.id));
            var serializableMax = serializableNodes.Count <= 0 ? 0 : serializableNodes.Max(x => x.Id);
            maxId    = serializableMax > maxId ? serializableMax : maxId;
            uniqueId = ++maxId;
            return uniqueId;
        }

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
        public T AddNode<T>(string newNodeName) where T : class, INode => AddNode(newNodeName, typeof(T)) as T;

        /// <summary>
        /// return node by it ID
        /// </summary>
        public INode GetNode(int nodeId)
        {
            _nodesCache = _nodesCache ?? new Dictionary<int, INode>();
            if (_nodesCache.Count != nodes.Count) {
                _nodesCache.Clear();
                _nodesCache = Nodes.ToDictionary(x => x.Id);
            }

            _nodesCache.TryGetValue(nodeId, out var node);
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

            node.SetId(GetId());
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
            if (string.IsNullOrEmpty(guid))
                guid = System.Guid.NewGuid().ToString();
            
            _allNodes?.Clear();
            
            nodes.Clear();
            
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

        private (bool isValid, int id) GetIdFromOriginSource()
        {
            var result = (false,0);
#if UNITY_EDITOR
            var isVariant = UnityEditor.PrefabUtility.IsPartOfVariantPrefab(this);
            if (!isVariant) return result;
            
            var origin = UnityEditor.PrefabUtility.GetCorrespondingObjectFromOriginalSource(gameObject);
            var graph = origin?.GetComponent<NodeGraph>();
            if (!graph)
                return result;

            var originId = graph.GetId();
            UnityEditor.EditorUtility.SetDirty(origin);

            result = (true, originId);
#endif
            return result;
        }
        
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

        /// <summary>
        /// finish node life time
        /// </summary>
        private void OnDisable()
        {
            if(exitOnDisable)
                Exit();
        }

        private void OnDestroy()
        {
            Exit();
        }

        private void OnEnable()
        {
            if(activateOnEnable && Application.isPlaying)
                Execute();
        }

        #endregion
        


    }
}