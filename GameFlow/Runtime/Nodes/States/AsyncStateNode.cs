using UniGame.UniNodes.NodeSystem.Runtime.Core.Nodes;

namespace UniModules.UniGameFlow.Nodes.Runtime.States
{
    using System;
    using System.Diagnostics;
    using Cysharp.Threading.Tasks;
    using global::UniCore.Runtime.ProfilerTools;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Attributes;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
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
    public abstract class AsyncStateNode : SNode,
        IAsyncContextState,
        IAsyncStateCommand<IContext, AsyncStatus>,
        IAsyncCompletion<AsyncStatus, IContext>,
        IAsyncEndPoint<IContext>,
        IAsyncRollback<IContext>,
        IStateCancellation
    {
        #region inspector

        [ReadOnlyValue] [SerializeField] protected bool _isStateActive;

        #endregion

        [Port] public object input;

        #region public properties

        #endregion

        #region private fields

        private AsyncContextStateProxy _asyncStateProxy;


        private IStateToken _token;
        private IPortValue  _inputPort;

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


        #region custom execution handlers

        /// <summary>
        /// Regular Execution logic behaviour
        /// </summary>
        public virtual UniTask<AsyncStatus> ExecuteStateAsync(IContext value) => UniTask.FromResult(AsyncStatus.Succeeded);

        /// <summary>
        /// state completion handler
        /// </summary>
        public virtual UniTask CompleteAsync(AsyncStatus value, IContext data, ILifeTime lifeTime) => UniTask.FromResult(UniTask.CompletedTask);

        /// <summary>
        /// Exiting from state handler
        /// </summary>
        public virtual UniTask ExitAsync(IContext data) => UniTask.FromResult(UniTask.CompletedTask);

        /// <summary>
        /// Execution Failure result handler
        /// </summary>
        public virtual UniTask Rollback(IContext source) => UniTask.FromResult(UniTask.CompletedTask);

        public void StopState() => ExitAsync();

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

        protected sealed override void OnExecute()
        {
            if (!Application.isPlaying)
                return;

            _isStateActive   = false;
            _asyncStateProxy = new AsyncContextStateProxy(this, this, this, this);
            _inputPort       = GetPortValue(nameof(input));

            LifeTime.AddCleanUpAction(() => _asyncStateProxy.ExitAsync());

            LogNodeExecutionState();

            //get all actual stat tokens and try to run state
            _inputPort.Receive<IStateToken>().Where(x => _isStateActive == false).Select(async x => await OwnToken(x)).Subscribe().AddTo(LifeTime);
        }


        [Conditional("UNITY_EDITOR")]
        private void LogNodeExecutionState()
        {
            _asyncStateProxy.Value.Do(x => GameLog.Log($"STATE NODE {ItemName} ID {Id} STATUS : {x}")).Subscribe().AddTo(LifeTime);
        }

        private async UniTask OwnToken(IStateToken token)
        {
            var result = token.TakeOwnership(this);
            _token         = result ? token : null;
            _isStateActive = result;
            await ExecuteAsync(token.Context);
        }
    }
}