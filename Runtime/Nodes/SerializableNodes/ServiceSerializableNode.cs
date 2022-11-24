using System;
using Cysharp.Threading.Tasks;
using UniCore.Runtime.ProfilerTools;
using UniGame.GameFlow.Runtime.Interfaces;
using UniGame.UniNodes.Nodes.Runtime.Common;
using UniModules.GameFlow.Runtime.Attributes;
using UniCore.Runtime.Attributes;
using UniModules.UniCore.Runtime.ProfilerTools;
using UniGame.Core.Runtime;
using UnityEngine;

namespace UniGame.UniNodes.GameFlow.Runtime.Nodes
{
    /// <summary>
    /// Base game service binder between Unity world and regular classes
    /// </summary>
    /// <typeparam name="TServiceApi"></typeparam>
    [HideNode]
    [Serializable]
    public abstract class ServiceSerializableNode<TServiceApi> : 
        SContextNode
        where TServiceApi :  IGameService
    {
        [SerializeField]
        private TServiceApi _service;

        #region inspector

        [ReadOnlyValue]
        [SerializeField]
        private bool isReady;
        
        #endregion

        public TServiceApi Service => _service;

        protected abstract UniTask<TServiceApi> CreateService(IContext context);

        protected sealed override async UniTask<bool> OnContextActivate(IContext context)
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

            isReady = true;
            
            await BindService(_service,context);
            await OnServiceCreated(_service,context);
            
            GameLog.LogRuntime($"NODE SERVICE {typeof(TServiceApi).Name} CREATED");

            return true;
        }

        protected virtual UniTask OnServiceCreated(TServiceApi service,IContext context) => UniTask.CompletedTask;
        
        private UniTask<IContext> BindService(TServiceApi service,IContext context)
        {
            context.Publish(service);
            CompleteProcessing(context);
            return UniTask.FromResult(context);
        }
    }
}