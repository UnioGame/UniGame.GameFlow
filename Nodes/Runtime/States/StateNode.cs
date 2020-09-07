using UniGame.UniNodes.NodeSystem.Runtime.Core.Nodes;

namespace UniModules.UniGameFlow.Nodes.Runtime.States
{
    using System;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Attributes;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Core;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
    using UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniGame.Core.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniRx;
    using UnityEngine;

    [Serializable]
    public class ReactiveStateToken : ReactivePortValue<StateToken>
    {
    }

    [Serializable]
    public class StateToken : ILifeTimeContext, IDisposable
    {
        private LifeTimeDefinition _lifeTime = new LifeTimeDefinition();
        public  int                Id => _lifeTime.Id;

        public StateNode PreviousState { get; protected set; }

        public ILifeTime LifeTime => _lifeTime;

        public bool TakeOwnership(StateNode state)
        {
            _lifeTime.Release();
            
        }

        public void Dispose() => _lifeTime.Terminate();
    }
    
    [HideNode]
    [Serializable]
    public abstract class StateNode : SNode
    {
        #region inspector

        [SerializeField]
        private bool isSingleOwner = true;
        
        #endregion
        
        private LifeTimeDefinition _stateLifetime;
        

        [Port]
        public ReactiveStateToken input = new ReactiveStateToken();

        #region public properties
        
        
        
        #endregion

        protected sealed override void OnInitialize()
        {
            _stateLifetime = _stateLifetime ?? new LifeTimeDefinition();
            _stateLifetime.Release();
            
            input.Initialize(this);
        }

        protected override void OnExecute()
        {
            
        }

        protected virtual void ExecuteState(ILifeTime stateLifeTime,StateToken token)
        {
            
        }
    }
}
