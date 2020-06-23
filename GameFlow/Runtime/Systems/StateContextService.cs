namespace UniModules.UniGameFlow.GameFlow.Runtime.Systems
{
    using System;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniStateMachine.Runtime.Interfaces;
    using UniRx;
    using UniRx.Async;
    using UnityEngine;

    public abstract class StateContextService : 
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
            serviceData.Execute(LifeTime);
            return this;
        }

        public void Release() => Exit();

        protected sealed override async UniTask<Unit> OnInitialize(IObservable<IContext> source)
        {
            source.
                Subscribe(UpdateContext).
                AddTo(LifeTime);

            serviceData.ExecuteServices(source,LifeTime);

            return Unit.Default;
        }

        
        protected virtual void UpdateContext(IContext context)
        {
            
        }
    }
}
