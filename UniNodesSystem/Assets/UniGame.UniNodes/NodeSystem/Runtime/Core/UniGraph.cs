namespace UniGreenModules.UniNodeSystem.Nodes
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Runtime.Core;
    using Runtime.Extensions;
    using Runtime.Interfaces;
    using Sirenix.Utilities;
    using UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniCore.Runtime.Rx.Extensions;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Attributes;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
    using UniRx;
    using UnityEngine;

    [HideNode]
    public class UniGraph : NodeGraph, IUniGraph
    {
        
        #region private properties

        /// <summary>
        /// graph cancelation
        /// </summary>
        private List<IGraphCancelationNode> cancelationNodes = new List<IGraphCancelationNode>();
        
        /// <summary>
        /// graph inputs
        /// </summary>
        private List<IGraphPortNode> inputs = new List<IGraphPortNode>();
        
        /// <summary>
        /// graph outputs
        /// </summary>
        private List<IGraphPortNode> outputs = new List<IGraphPortNode>();

        /// <summary>
        /// all child nodes
        /// </summary>
        private List<IUniNode> allNodes = new List<IUniNode>();

        #endregion

        public GameObject AssetInstance => gameObject;

        public IReadOnlyList<IGraphPortNode> OutputsPorts => outputs;
        
        public IReadOnlyList<IGraphPortNode> InputsPorts => inputs;
        
        public override void Dispose() => Exit();
        
        #region private methods

        protected override void OnInitialize()
        {
            InitializeGraphNodes();
        }

        protected override void OnExecute()
        {
            ActiveGraphs.Add(this);

            LifeTime.AddCleanUpAction(() => ActiveGraphs.Remove(this));

            for (var i = 0; i < cancelationNodes.Count; i++) {
                var x = cancelationNodes[i];
                x.PortValue.PortValueChanged.
                    Subscribe(unit => Exit()).
                    AddTo(LifeTime);
            }
                      
            inputs.ForEach(x => BindConnections(this,GetPort(x.ItemName),x.PortValue) );
            outputs.ForEach(x => BindConnections(this,GetPort(x.ItemName),x.PortValue));

            for (int i = 0; i < allNodes.Count; i++) {
                var node = allNodes[i];
                node.Ports.ForEach(x => BindConnections(node, x, x.Value));
            }
            
            allNodes.ForEach( x => x.Execute());

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BindConnections(IUniNode node,INodePort sourcePort,IMessagePublisher publisher)
        {
            //data source connections allowed only for input ports
            if (sourcePort.Direction != PortIO.Input) {
                return;
            }

            var connections = sourcePort.GetConnections();
            
            for (var i = 0; i < connections.Count; i++) {
                var connection = connections[i];
                var port       = connection;
                if(port.Direction == PortIO.Input || port.Id == Id)
                    continue;
                
                var value = connection.Value;
                value.Bind(publisher).
                    AddTo(LifeTime);
            }

            connections.DespawnCollection();
        }

        private void InitializeGraphNodes()
        {
            allNodes.Clear();
            cancelationNodes.Clear();
            inputs.Clear();
            outputs.Clear();
            
            for (var i = 0; i < nodes.Count; i++) {

                var node = nodes[i];
                
                //skip all not unigraph nodes
                if (!(node is IUniNode uniNode))
                    continue;

                //register graph ports by nodes
                UpdatePortNode(uniNode);

                //stop graph execution, if cancelation node output triggered
                if (uniNode is IGraphCancelationNode cancelationNode) {
                    cancelationNodes.Add(cancelationNode);
                }

                uniNode.Initialize(this);
                
                lifeTime.AddCleanUpAction(uniNode.Exit);
                
                allNodes.Add(uniNode);
            }
        }
        
        private void UpdatePortNode(IUniNode uniNode)
        {
            //register input/output nodes
            if (!(uniNode is IGraphPortNode graphPortNode)) {
                return;
            }

            var container = graphPortNode.Direction == PortIO.Input ? 
                inputs : outputs;
      
            //add graph ports for exists port nodes
            this.UpdatePortValue(graphPortNode.ItemName, graphPortNode.Direction);
               
            container.Add(graphPortNode);

        }

        private void OnDisable() => Dispose();
        
        #endregion
        
        
    }
}