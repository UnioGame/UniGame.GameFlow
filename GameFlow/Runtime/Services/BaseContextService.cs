namespace UniModules.UniGameFlow.GameFlow.Runtime.Services
{
    using System;
    using Systems;
    using UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniStateMachine.Runtime.Interfaces;
    using UniRx;
    using UniRx.Async;
    using UnityEngine;

    public abstract class BaseContextService : 
        BaseServiceAsset<IObservable<IContext>>,
        IAsyncState<IDisposable>
    {
        #region inspector

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineProperty]
        [Sirenix.OdinInspector.HideLabel]
#endif
        [SerializeField]
        public ContextServiceData serviceData = new ContextServiceData();
        
        #endregion

        public async UniTask<IDisposable> Execute()
        {
            var contextObservable = await serviceData.LoadDataSource(LifeTime);
            return await Execute(contextObservable);
        }

        public void Release() => Exit();

        protected sealed override async UniTask<Unit> OnInitialize(IObservable<IContext> source)
        {
            source.
                Where(x => x!=null).
                Subscribe(UpdateContext).
                AddTo(LifeTime);

            return await serviceData.ExecuteServices(source,LifeTime);
        }

        
        protected virtual void UpdateContext(IContext context)
        {
            GameLog.Log($"ContextService {name} UpdateContext");
        }
    }
}
