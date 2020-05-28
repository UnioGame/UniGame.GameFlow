namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using Attributes;
    using Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;

    [HideNode]
    [Serializable]
    public abstract class UniNode : Node, IUniNode
    {
        #region public properties

        /// <summary>
        /// Is node currently active
        /// </summary>
        public bool IsActive => SNode.IsActive;

        public ILifeTime LifeTime => SNode.LifeTime;

        #endregion

        #region public methods

        public sealed override void Initialize(IGraphData graphData)
        {
            base.Initialize(graphData);
            SNode.Initialize(graphData,OnInitialize, UpdateCommands, OnExecute);
        }

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
        /// finish node life time
        /// </summary>
        private void OnDisable() => Exit();

    }
}