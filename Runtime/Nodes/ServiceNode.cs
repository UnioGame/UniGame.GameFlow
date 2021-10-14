namespace UniGame.UniNodes.GameFlow.Runtime.Nodes
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.Attributes;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGameFlow.GameFlow.Runtime.Interfaces;
    using UniNodes.Nodes.Runtime.Common;
    using UniRx;
    
    using UnityEngine;

    /// <summary>
    /// Base game service binder between Unity world and regular classes
    /// </summary>
    /// <typeparam name="TServiceApi"></typeparam>
    [HideNode]
    public abstract class ServiceNode<TServiceApi> : 
        ContextNode
        where TServiceApi :  IGameService
    {
        [SerializeField]
        private TServiceApi _service;

        #region inspector

        [ReadOnlyValue]
        [SerializeField]
        private bool _isReady;
        
        #endregion
        
        private IDisposable _serviceDisposable;

        public bool waitForServiceReady = true;

        public TServiceApi Service => _service;

        protected abstract UniTask<TServiceApi> CreateService(IContext context);

        protected sealed override UniTask OnExecute()
        {
            Source.Where(x => x != null).
                Do(async x => await OnContextAvailable(x)
                    .AttachExternalCancellation(LifeTime.TokenSource)
                    .SuppressCancellationThrow()).
                Subscribe().
                AddTo(LifeTime);
            
            return  UniTask.CompletedTask;
        }

        protected virtual void OnServiceCreated(IContext context)
        {
        }

        private async UniTask<IContext> OnContextAvailable(IContext context)
        {
            var serviceLifeTime = context.LifeTime.Compose(LifeTime);
            
            _service = await CreateService(context);
            _service.AddTo<IDisposable>(serviceLifeTime);

            await BindService(context);
            
            OnServiceCreated(context);
            
            GameLog.LogRuntime($"NODE SERVICE {typeof(TServiceApi).Name} CREATED");
            return context;
        }
        
        private UniTask<IContext> BindService(IContext context)
        {
            _serviceDisposable = _service.IsReady.
                Where(x => x || !waitForServiceReady).
                Do(_ => context.Publish<TServiceApi>(_service)).
                Do(_ => Complete()).
                Subscribe().
                AddTo(LifeTime);
            
            return UniTask.FromResult(context);
        }
    }
}