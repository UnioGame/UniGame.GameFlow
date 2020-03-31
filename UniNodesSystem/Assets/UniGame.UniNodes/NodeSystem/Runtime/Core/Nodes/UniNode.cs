namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using Attributes;
    using Nodes;
    using Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;

    [HideNode]
    [Serializable]
    public abstract class UniNode : Node, IUniNode
    {       
        
        private IProxyNode serializableNode;

        
        #region public properties
        
        /// <summary>
        /// regular source node
        /// </summary>
        protected IProxyNode SNode => GetSourceNode();

        /// <summary>
        /// Is node currently active
        /// </summary>
        public bool IsActive => SNode.IsActive;

        public ILifeTime LifeTime => SNode.LifeTime;

        public IReadOnlyCollection<INodePort> PortValues => SNode.PortValues;

        #endregion

        #region public methods

        public sealed override void Initialize(IGraphData graphData)
        {
            graph = graphData;
            SNode.Bind(OnInitialize, UpdateCommands, OnExecute);
            SNode.Initialize(graphData);
        }

        public override bool AddPortValue(INodePort portValue) => SNode.AddPortValue(portValue);
        
        /// <summary>
        /// stop execution
        /// </summary>
        public void Exit() => SNode.Exit();

        /// <summary>
        /// start node execution
        /// </summary>
        public void Execute()
        {
            Initialize(GraphData);
            
            SNode.Execute();
        }

        /// <summary>
        /// stop node execution
        /// </summary>
        public void Release() => Exit();

        public override void Validate() => SNode.Validate();
        
        #endregion

        /// <summary>
        /// Call once on node initialization
        /// </summary>
        protected virtual void OnInitialize(){}

        /// <summary>
        /// base logic realization
        /// </summary>
        protected virtual void OnExecute(){}

        /// <summary>
        /// update active list commands
        /// add all supported node commands here
        /// </summary>
        protected virtual void UpdateCommands(List<ILifeTimeCommand> nodeCommands){}

        /// <summary>
        /// create base node realization
        /// </summary>
        protected virtual IProxyNode CreateInnerNode() => new SNode(id, nodeName, ports);

        /// <summary>
        /// create target source node and bind with mono node methods
        /// </summary>
        /// <returns></returns>
        private IProxyNode GetSourceNode()
        {
            if (serializableNode != null)
                return serializableNode;
            serializableNode = CreateInnerNode();
            return serializableNode;
        }
        
        /// <summary>
        /// finish node life time
        /// </summary>
        private void OnDisable() => Exit();

    }
}