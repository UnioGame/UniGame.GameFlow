namespace UniGreenModules.UniNodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using UniCore.Runtime.Attributes;
    using UniGame.Core.Runtime.DataStructure;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Nodes;
    using UnityEngine;

    [Serializable]
    public class NodesDictionary : SerializableDictionary<ulong, Node>
    {
    }

    /// <summary> Base class for all node graphs </summary>
    [Serializable]
    public abstract class NodeGraph : UniNode, IDisposable , IGraphData
    {
        #region static data

        public static HashSet<NodeGraph> ActiveGraphs { get; } = new HashSet<NodeGraph>();

        private Dictionary<int, Node> nodesCache;

        #endregion

        [ReadOnlyValue] [SerializeField] private int uniqueId;

        /// <summary> All nodes in the graph. <para/>
        /// See: <see cref="AddNode{T}"/> </summary>
        [SerializeField]
        public List<Node> nodes = new List<Node>();

        #region public properties

        public IReadOnlyList<INode> Nodes => nodes;

        public sealed override IGraphData Graph => this;

        #endregion

        #region graph operations

        public int GetId() => ++uniqueId;

        public int UpdateId(int oldId)
        {
            return GetId();
        }
        
        /// <summary> Add a node to the graph by type </summary>
        public T AddNode<T>() where T : Node => AddNode(typeof(T)) as T;

        public T AddNode<T>(string name) where T : Node => AddNode(name, typeof(T)) as T;

        public Node GetNode(int nodeId)
        {
            nodesCache = nodesCache ?? new Dictionary<int, Node>();
            if (nodesCache.Count != nodes.Count) {
                nodesCache.Clear();
                nodesCache = nodes.ToDictionary(x => x.Id);
            }

            nodesCache.TryGetValue(nodeId, out var node);
            return node;
        }

        
        public new void SetGraph(NodeGraph parent)
        {
        }

        public virtual Node AddNode(string itemName, Type type)
        {
            var nodeAsset = gameObject.AddComponent(type);
            var node      = nodeAsset as Node;
            if (node == null) {
                DestroyImmediate(nodeAsset, true);
                return null;
            }

            node.SetGraph(this);
            node.graph = graph;
            node.nodeName = itemName;
            nodes.Add(node);

            return node;
        }

        /// <summary> Add a node to the graph by type </summary>
        public Node AddNode(Type type) => AddNode(type.Name, type);

        /// <summary> Creates a copy of the original node in the graph </summary>
        public virtual Node CopyNode(Node original)
        {
            var node = Instantiate(original);
            node.UpdateId();
            node.ClearConnections();
            nodes.Add(node);
            node.SetGraph(this);
            return node;
        }

        /// <summary> Safely remove a node and all its connections </summary>
        /// <param name="node"> The node to remove </param>
        public void RemoveNode(Node node)
        {
            node.ClearConnections();
            nodes.Remove(node);
            if (Application.isPlaying) 
                Destroy(node);
        }

        /// <summary> Remove all nodes and connections from the graph </summary>
        public void Clear()
        {
            if (Application.isPlaying) {
                for (var i = 0; i < nodes.Count; i++) {
                    Destroy(nodes[i]);
                }
            }

            nodesCache.Clear();
            nodes.Clear();
        }

        /// <summary> Create a new deep copy of this graph </summary>
        public NodeGraph Copy()
        {
            // Instantiate a new nodegraph instance
            var graph = Instantiate(this);
            // Instantiate all nodes inside the graph
            for (var i = 0; i < nodes.Count; i++) {
                if (nodes[i] == null) continue;
                var node = Instantiate(nodes[i]) as Node;
                node.SetGraph(this);
                graph.nodes[i] = node;
            }

            // Redirect all connections
            for (var i = 0; i < graph.nodes.Count; i++) {
                if (graph.nodes[i] == null) continue;
                foreach (var port in graph.nodes[i].Ports) {
                    port.Redirect(nodes, graph.nodes);
                }
            }

            return graph;
        }

        public virtual void Dispose()
        {
        }

        #endregion

        private void OnDestroy() => Clear();
    }
}