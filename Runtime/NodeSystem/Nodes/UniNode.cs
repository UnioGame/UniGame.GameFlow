namespace UniModules.GameFlow.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using Attributes;
    using Cysharp.Threading.Tasks;
    using Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;

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

        public sealed override void Initialize(NodeGraph graphData)
        {
            base.Initialize(graphData);
            SNode.Initialize(GraphData,OnInitialize, UpdateCommands, OnExecute);
        }

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

        public override void Validate() => SNode.Validate();
        
        #endregion

        
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

    }
}