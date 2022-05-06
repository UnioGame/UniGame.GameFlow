namespace UniModules.UniGameFlow.Nodes.Runtime.States
{
    using System;
    using System.Diagnostics;
    using Cysharp.Threading.Tasks;
    using global::UniCore.Runtime.ProfilerTools;
    using global::UniModules.GameFlow.Runtime.Attributes;
    using global::UniModules.GameFlow.Runtime.Core;
    using global::UniModules.GameFlow.Runtime.Interfaces;
    using UniCore.Runtime.Attributes;
    using UniGame.Context.SerializableContext.Runtime.Abstract;
    using UniGame.Context.SerializableContext.Runtime.States;
    using UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniGame.Core.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UnityEngine;

    [HideNode]
    [Serializable]
    public abstract class AsyncStateUniNode : UniNode,
        IAsyncContextState,
        IAsyncStateCommand<IContext, AsyncStatus>,
        IAsyncCompletion<AsyncStatus, IContext>,
        IAsyncEndPoint<IContext>,
        IAsyncRollback<IContext>,
        IStateCancellation
    {
        #region inspector

        [ReadOnlyValue] 
        [SerializeField] 
        protected bool _isStateActive;

        #endregion

        [Port] public object input;

        #region public properties

        #endregion

        #region private fields

        private AsyncContextStateProxy _asyncStateProxy;
        
        private IStateToken _token;
        private IPortValue       _inputPort;

        #endregion

        public bool IsStateActive => _isStateActive;

        public IStateToken Token => _token;
        
        
        #region public methods

        public async UniTask<AsyncStatus> ExecuteAsync(IContext value) => await _asyncStateProxy.ExecuteAsync(value);

        public async UniTask ExitAsync()
        {
            _isStateActive = false;
            _token         = null;
            await _asyncStateProxy.ExitAsync();
        }

        public void StopState() => ExitAsync();
        
        #region custom execution handlers
        
        public virtual UniTask<AsyncStatus> ExecuteStateAsync(IContext value) => UniTask.FromResult(AsyncStatus.Succeeded);

        public virtual UniTask CompleteAsync(AsyncStatus value, IContext data, ILifeTime lifeTime) => UniTask.FromResult(UniTask.CompletedTask);

        public virtual UniTask ExitAsync(IContext data) => UniTask.FromResult(UniTask.CompletedTask);

        public virtual UniTask Rollback(IContext source) => UniTask.FromResult(UniTask.CompletedTask);

        #endregion
        
        #endregion

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
            
            _isStateActive   = false;
            _asyncStateProxy = new AsyncContextStateProxy(this, this, this, this);
            _inputPort       = GetPortValue(nameof(input));

            LifeTime.AddCleanUpAction(() => _asyncStateProxy.ExitAsync());

            LogNodeExecutionState();

            //get all actual stat tokens and try to run state
            _inputPort.Receive<IStateToken>().
                Where(x => _isStateActive == false).
                Select(async x => await OwnToken(x)).
                RxSubscribe().
                AddTo(LifeTime);

            return UniTask.CompletedTask;
        }
        
        [Conditional("UNITY_EDITOR")]
        private void LogNodeExecutionState()
        {
            _asyncStateProxy.Value
                .Do(x => GameLog.Log($"STATE NODE {ItemName} ID {Id} STATUS : {x}"))
                .RxSubscribe()
                .AddTo(LifeTime);
        }

        private async UniTask OwnToken(IStateToken token)
        {
            var result = token.TakeOwnership(this);
            _token         = result ? token : null;
            _isStateActive = result;
            
            if (_token == null)
                return;

            await ExecuteAsync(_token.Context);
        }

    }
}