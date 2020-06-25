namespace UniGame.UniNodes.GameFlow.Runtime.Nodes
{
    using System;
    using NodeSystem.Runtime.Attributes;
    using UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.Runtime.Attributes;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGameFlow.GameFlow.Runtime.Interfaces;
    using UniNodes.Nodes.Runtime.Common;
    using UniRx;
    using UniRx.Async;
    using UnityEngine;

    /// <summary>
    /// Base game service binder between Unity world and regular classes
    /// </summary>
    /// <typeparam name="TServiceApi"></typeparam>
    [HideNode]
    public abstract class ServiceNode<TServiceApi> : 
        ContextNode
        where TServiceApi : IGameService
    {
        [SerializeField]
        private TServiceApi _service;

        #region inspector

        [Header("Service Status")]
        [ReadOnlyValue]
        [SerializeField]
        private bool _isReady;
        
        #endregion
        
        private IDisposable _serviceDisposable;

        public bool waitForServiceReady = true;

        public TServiceApi Service => _service;

        protected abstract UniTask<TServiceApi> CreateService(IContext context);

        protected sealed override void OnExecute()
        {
            Source.Where(x => x != null).
                Do(async x => await OnContextAvailable(x)).
                Subscribe().
                AddTo(LifeTime);
        }

        protected virtual void OnServiceCreated(IContext context)
        {
        }

        private async UniTask<IContext> OnContextAvailable(IContext context)
        {
            _service = await CreateService(context);
            
            LifeTime.AddDispose(_service);
            
            await BindService(context);
            OnServiceCreated(context);
            GameLog.LogRuntime($"NODE SERVICE {typeof(TServiceApi).Name} CREATED");
            return context;
        }
        
        private async UniTask<IContext> BindService(IContext context)
        {
            _service.Bind(context);
            
            _serviceDisposable?.Dispose();

            _serviceDisposable = _service.IsReady.
                Where(x => x || !waitForServiceReady).
                Do(_ => context.Publish<TServiceApi>(_service)).
                Do(_ => Complete()).
                Subscribe().
                AddTo(LifeTime);
            
            return context;
        }
    }
}