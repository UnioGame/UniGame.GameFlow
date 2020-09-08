using UniGame.UniNodes.NodeSystem.Runtime.Core.Nodes;

namespace UniModules.UniGameFlow.Nodes.Runtime.States
{
    using System;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Attributes;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Core;
    using UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniGame.Core.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniGame.Core.Runtime.Rx;
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

        public StateNode ActiveState { get; protected set; }

        public ILifeTime LifeTime => _lifeTime;

        public bool TakeOwnership(StateNode state)
        {
            if (state.IsSingleState) {
                _lifeTime.Release(); 
            }

            PreviousState = ActiveState;
            ActiveState   = state;
            return true;
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
        
        [SerializeField]
        private BoolRecycleReactiveProperty _isActive = new BoolRecycleReactiveProperty();

        #endregion
        
        [Port]
        public ReactiveStateToken input = new ReactiveStateToken();

        #region public properties

        public IReadOnlyReactiveProperty<bool> IsStateActive => _isActive;
        
        public bool IsSingleState => isSingleOwner;

        #endregion

        protected sealed override void OnInitialize()
        {
            input.Initialize(this);
        }

        protected sealed override void OnExecute()
        {
            input.Where(x => x.TakeOwnership(this) && _isActive.Value == false).
                Do(x => _isActive.Value = true).
                Do(x => x.LifeTime.AddCleanUpAction(() => _isActive.Value = false)).
                Subscribe(ExecuteState).
                AddTo(LifeTime);
        }

        protected abstract void ExecuteState(StateToken token);
    }
}
