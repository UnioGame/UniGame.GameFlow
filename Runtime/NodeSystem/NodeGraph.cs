using UniModules.GameFlow.Runtime.Attributes;
using UniModules.UniGame.Context.Runtime.Connections;

namespace UniModules.GameFlow.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Runtime.Extensions;
    using Runtime.Interfaces;
    using global::UniCore.Runtime.Attributes;
    using global::UniGame.Runtime.ObjectPool;
    using global::UniGame.Runtime.ObjectPool.Extensions;
    using UnityEngine;
    using Object = UnityEngine.Object;

#if UNITY_EDITOR
    using UnityEditor;
    using UniModules.Editor;
#endif

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    /// <summary> Base class for all node graphs </summary>
    [Serializable]
    [HideNode]
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
        [SerializeField] 
        public int uniqueId;

        //[HideInInspector]
        [SerializeField]
        private int _nextId = 0;

#if ODIN_INSPECTOR
        [InlineEditor(Expanded = false)]
        [Searchable]
#endif
        [SerializeField]
        public List<UniNode> nodes = new List<UniNode>();
  
#if ODIN_INSPECTOR
        [InlineProperty]
#endif
        [SerializeReference]
        public List<INode> serializableNodes = new List<INode>();

        #endregion

        [NonSerialized]
        private NodeGraph _originSource;

        private List<INode> _allNodes = new List<INode>();

        #region public properties

        public Transform Root => transform;
        
        public abstract IContextConnection GraphContext { get; }

        public override string ItemName => name;

        public IReadOnlyList<INode> Nodes => GetNodes();

        public IReadOnlyList<INode> SerializableNodes => serializableNodes;
        
        public IReadOnlyList<INode> ObjectNodes => nodes;

        public sealed override NodeGraph GraphData => this;

        public string Guid => guid;
        
        #endregion

        public void Dispose() => Exit();

        #region graph operations

        /// <summary>
        /// get unique Id in graph scope
        /// </summary>
        /// <returns></returns>
        public int GetNextId()
        {
            var activeId = _nextId;
            _nextId++;
#if UNITY_EDITOR
            gameObject.MarkDirty();
#endif
            return activeId;
        }

        public int GetId() => uniqueId;
        
        public int UpdateId(int oldId) => GetNextId();

        public IReadOnlyList<INode> GetNodes()
        {
            _allNodes ??= new List<INode>();
            if (_allNodes.Count > 0)
                return _allNodes;

            if(nodes!=null)
                _allNodes.AddRange(nodes);
            if(serializableNodes !=null)
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
        public INode GetNode(int nodeId) => Nodes.FirstOrDefault(x => x.Id == nodeId);

        /// <summary>
        /// add node by type
        /// </summary>
        public virtual INode AddNode(string itemName, Type type)
        {
            var node = objectType.IsAssignableFrom(type) ? 
                AddAssetNode(type) : 
                AddSerializableNode(type);

            if (node == null) return null;

            var nextId = GetNextId();
            node.SetId(nextId);
            node.SetUpData(this);
            node.Initialize(this);
            node.SetName(itemName);
            node.UpdateNodePorts();
            
            _allNodes?.Add(node);
            
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
        public virtual UniNode CopyNode(UniNode original)
        {
            var node = Instantiate(original);
            node.SetId(GetNextId());
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

            var nodeAsset = node as UniNode;
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
            _allNodes?.Clear();

            OnInnerValidate();
            
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

        protected virtual IEnumerable<INode> GetCustomNodes()
        {
            yield break;
        }

        protected virtual void OnInnerValidate()
        {
            if (string.IsNullOrEmpty(guid))
                guid = System.Guid.NewGuid().ToString();

            serializableNodes.RemoveAll(x => x == null || x is Object);
            nodes.RemoveAll(x => !x);
            nodes.RemoveAll(x => x == null);
            nodes.Remove(this);
            
            foreach (var childNode in GetComponents<UniNode>())
            {
                if(nodes.Contains(childNode) || childNode == this) continue;
                nodes.Add(childNode);
            }
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
                case UniNode nodeAsset:
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

        private void OnDestroy() => Exit();


        #endregion
        
        
    }
}