namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Attributes;
    using Runtime.Extensions;
    using Runtime.Interfaces;
    using Sirenix.Utilities;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UnityEngine;

    [HideNode]
    public class UniGraph : NodeGraph, IUniGraph
    {
        
        #region private properties

        /// <summary>
        /// graph cancellation
        /// </summary>
        private List<IGraphCancelationNode> cancellationNodes = new List<IGraphCancelationNode>();
        
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
        private List<IUniNode> uniNodes = new List<IUniNode>();

        #endregion

        public GameObject AssetInstance => gameObject;

        public IReadOnlyList<IGraphPortNode> OutputsPorts => outputs;
        
        public IReadOnlyList<IGraphPortNode> InputsPorts => inputs;

        public void Initialize()
        {
            Initialize(this);
        }
        
        public override void Dispose() => Exit();

        #region private methods

        protected sealed override void OnInitialize()
        {
            base.OnInitialize();
            InitializeGraphNodes();
               
#if UNITY_EDITOR
            if (Application.isPlaying == false) {
                Validate();
            }
#endif
        }

        protected override void OnExecute()
        {
            LifeTime.AddCleanUpAction(() => ActiveGraphs.Remove(this));

            for (var i = 0; i < cancellationNodes.Count; i++) {
                var x = cancellationNodes[i];
                x.PortValue.PortValueChanged.
                    Subscribe(unit => Exit()).
                    AddTo(LifeTime);
            }
                      
            inputs.ForEach(x => BindConnections(this,GetPort(x.ItemName),x.PortValue) );
            outputs.ForEach(x => BindConnections(this,GetPort(x.ItemName),x.PortValue));

            //bind all ports and only after that start execution
            for (int i = 0; i < uniNodes.Count; i++) {
                var node = uniNodes[i];
                node.Ports.ForEach(x => BindConnections(node, x, x.Value));
            }
            
            uniNodes.ForEach( x => x.Execute());

            ActiveGraphs.Add(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BindConnections(IUniNode node,INodePort sourcePort,IMessagePublisher publisher)
        {
            //data source connections allowed only for input ports
            if (sourcePort.Direction != PortIO.Input) {
                return;
            }

            var connections = sourcePort.Connections;
            
            for (var i = 0; i < connections.Count; i++) {
                var connection = connections[i];
                var port       = connection.Port;
                if(port.Direction == PortIO.Input || port.NodeId == Id)
                    continue;
                
                var value = port.Value;
                value.Bind(publisher).
                    AddTo(LifeTime);
            }
        }

        private void InitializeGraphNodes()
        {
            uniNodes.Clear();
            cancellationNodes.Clear();
            inputs.Clear();
            outputs.Clear();
            
            for (var i = 0; i < Nodes.Count; i++) {

                var node = Nodes[i];
                
                //register graph ports by nodes
                UpdatePortNode(node);

                //stop graph execution, if cancelation node output triggered
                if (node is IGraphCancelationNode cancelationNode) {
                    cancellationNodes.Add(cancelationNode);
                }
                
                //initialize node
                node.Initialize(this);
                
#if UNITY_EDITOR
                //Editor Only
                node.UpdatePortByAttributes();
#endif
                
                if (node is IUniNode uniNode) {
                    LifeTime.AddCleanUpAction(uniNode.Exit);
                    uniNodes.Add(uniNode);
                }

            }
        }
        
        private void UpdatePortNode(INode uniNode)
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

        #endregion
        
        
    }
}