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
        IAsyncCompletion<AsyncStatus,IContext> ,
        IAsyncEndPoint<IContext> ,
        IAsyncRollback<IContext>
    {
        #region inspector

        [ReadOnlyValue] [SerializeField] 
        protected bool _isStateActive;

        #endregion

        [Port] public object input;

        #region public properties

        #endregion

        #region private fields


        private AsyncContextStateProxy _asyncStateProxy;
        

        private IAsyncStateToken _token;

        #endregion

        public bool IsStateActive => _isStateActive;
        

        #region async state api methods
        
        public virtual async UniTask<AsyncStatus> ExecuteAsync(IContext value) => AsyncStatus.Succeeded;
        
        public virtual async UniTask              ExitAsync()                  => await UniTask.CompletedTask;

        public virtual async UniTask CompleteAsync(AsyncStatus value, IContext data, ILifeTime lifeTime) => await UniTask.CompletedTask;

        public virtual async UniTask ExitAsync(IContext data) => await UniTask.CompletedTask;

        public virtual async UniTask Rollback(IContext source) => await UniTask.CompletedTask;
        
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
            
            _isStateActive = false;
            
            LogNodeExecutionState();
            
            //get all actual stat tokens and try to run state
            var tokenPort = GetPortValue(nameof(input));
            tokenPort.Receive<IAsyncStateToken>().
                Where( x => _isStateActive == false).
                Select(async x =>  await OwnToken(x)).
                Subscribe().
                AddTo(LifeTime);
        }

        [Conditional("UNITY_EDITOR")]
        private void LogNodeExecutionState()
        {
            _asyncStateProxy.Value.
                Do(x => GameLog.Log($"STATE NODE {ItemName} ID {Id} STATUS : {x}")).
                Subscribe().
                AddTo(LifeTime);
        }
        
        protected sealed override void OnInitialize()
        {
            _asyncStateProxy = _asyncStateProxy ?? new AsyncContextStateProxy(this, this, this, this);
            _isStateActive   = false;
            
            LifeTime.AddCleanUpAction(() => _asyncStateProxy.ExitAsync());
        }

        private async UniTask OwnToken(IAsyncStateToken token)
        {
            var result = await token.TakeOwnership(_asyncStateProxy);
            _token         = result ? token : null;
            _isStateActive = result;
        }

    }
}