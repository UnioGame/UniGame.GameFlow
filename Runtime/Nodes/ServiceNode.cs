namespace UniGame.UniNodes.GameFlow.Runtime.Nodes
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.Attributes;
    using UniModules.UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGameFlow.GameFlow.Runtime.Interfaces;
    using UniModules.UniGameFlow.GameFlow.Runtime.Services;
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
        private bool isReady;

        public bool waitServiceReady = true;
        
        #endregion

        public bool waitForServiceReady = true;

        public TServiceApi Service => _service;

        protected abstract UniTask<TServiceApi> CreateService(IContext context);

        protected sealed override async UniTask OnContextActivate(IContext context)
        {
            
#if UNITY_EDITOR || GAME_LOGS_ENABLED
            var profileId = ProfilerUtils.BeginWatch($"Service_{typeof(TServiceApi).Name}");
            GameLog.Log($"GameService Profiler Init : {typeof(TServiceApi).Name} | {DateTime.Now}");
#endif 
            
            _service = await CreateService(context);
            
#if UNITY_EDITOR || GAME_LOGS_ENABLED
            var watchResult = ProfilerUtils.GetWatchData(profileId);
            GameLog.Log($"GameService Profiler Create : {typeof(TServiceApi).Name} | Take {watchResult.watchMs} | {DateTime.Now}");
#endif
            
            _service.AddTo(LifeTime);

            await BindService(_service,context);
            await OnServiceCreated(_service,context);
            
            GameLog.LogRuntime($"NODE SERVICE {typeof(TServiceApi).Name} CREATED");
        }

        protected virtual UniTask OnServiceCreated(TServiceApi service,IContext context) => UniTask.CompletedTask;
        
        private UniTask<IContext> BindService(TServiceApi service,IContext context)
        {
            if (!waitServiceReady)
                return UniTask.FromResult(context);
            
            service.IsReady
#if UNITY_EDITOR
                .Do(x => isReady = x)
#endif
                .Where(x => x || !waitForServiceReady)
                .Do(_ => context.Publish<TServiceApi>(_service))
                .Do(_ => Complete())
                .Subscribe()
                .AddTo(LifeTime);
            
            return UniTask.FromResult(context);
        }
    }
}