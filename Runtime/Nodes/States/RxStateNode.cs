using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
using UnityEngine;

namespace UniModules.UniGame.GameFlow.GameFlow.Runtime.Nodes.States
{
    using System;
    using System.Diagnostics;
    using Context.SerializableContext.Runtime.States;
    using Core.Runtime.DataFlow.Interfaces;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;
    using global::UniCore.Runtime.ProfilerTools;
    using global::UniModules.GameFlow.Runtime.Attributes;
    using global::UniModules.GameFlow.Runtime.Core;
    using global::UniModules.GameFlow.Runtime.Interfaces;
    using UniCore.Runtime.Attributes;
    using UniCore.Runtime.Rx.Extensions;
    using UniGameFlow.Nodes.Runtime.States;
    using UniRx;

    [CreateNodeMenu("Common/States/RxStateNode")]
    public class RxStateNode : UniNode,
        IRxStateExecution<IContext, Unit>,
        IRxCompletion<IContext, Unit>,
        IRxEndPoint,
        IRxRolldback<IContext>,
        IStateCancellation
    {
        #region inspector

        [ReadOnlyValue] [SerializeField] 
        protected bool _isStateActive;
        
        [SerializeField] 
        public bool autoRestart = true;

        #endregion

        [Port] public object input;

        #region public properties

        #endregion

        #region private fields

        private RxStateProxy<IContext> _state;
        private IStateToken            _token;
        private IPortValue             _inputPort;

        #endregion

        public bool IsStateActive => _isStateActive;

        public IStateToken Token => _token;

        public ILifeTime StateLifeTime => _state.LifeTime;

        #region proxy states

        public virtual IObservable<Unit> ExecuteState(IContext data, ILifeTime lifeTime) => Observable.Return(Unit.Default);

        public virtual IObservable<Unit> CompleteAsync(IContext data, Unit value, ILifeTime lifeTime) => Observable.Return(Unit.Default);

        public virtual IObservable<bool> Rollback(IContext data) => Observable.Return(true);

        public virtual void ExitState() { }

        #endregion

        public void StopState()
        {
            _isStateActive = false;
            _token         = null;
            _state.ExitState();
        }

        protected void PublishToken(INodePort port)
        {
            if (!_isStateActive || _token == null)
            {
                GameLog.LogWarning($"{ItemName} Try to Publish Token in inactive state");
                return;
            }

            port.Value.Publish(_token);
        }

        protected sealed override UniTask OnExecute()
        {
            if (!Application.isPlaying)
                return UniTask.CompletedTask;

            _isStateActive = false;
            _state         = new RxStateProxy<IContext>(this, this, this, this);
            _inputPort     = GetPortValue(nameof(input));

            LifeTime.AddCleanUpAction(StopState);

            LogNodeExecutionState();

            //get all actual stat tokens and try to run state
            _inputPort.Receive<IStateToken>()
                .Where(x => !_isStateActive)
                .Select(x => (token:x,owned:OwnToken(x)))
                .Where(x => x.owned)
                .Do(x => OnActivateState(x.token))
                .Subscribe()
                .AddTo(LifeTime);
            
            return UniTask.CompletedTask;
        }


        [Conditional("UNITY_EDITOR")]
        private void LogNodeExecutionState()
        {
            _state.Value.Do(x => GameLog.Log($"STATE NODE {ItemName} ID {Id} STATUS : {x}")).Subscribe().AddTo(LifeTime);
        }

        private bool OwnToken(IStateToken token)
        {
            var result = token.TakeOwnership(this);
            
            if (result && autoRestart)
                _state.ExitState();
            
            _token         = result ? token : null;
            _isStateActive = result;
            return result;
        }

        private void OnActivateState(IStateToken token)
        {
            _state.Execute(token.Context)
                .Subscribe()
                .AddTo(_state.LifeTime);
        }
    }
}