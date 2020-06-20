namespace UniModules.UniGameFlow.GameFlow.Runtime.Systems
{
    using System;
    using Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UniRx.Async;

    public abstract class ContextService : BaseServiceAsset<IObservable<IContext>>
    {
        #region inspector
        
        public bool isSharedSystem = true;

        #endregion

        private IGameService _sharedService;

        protected sealed override async UniTask<Unit> OnInitialize(IObservable<IContext> source)
        {
            source?.DistinctUntilChanged().
                Subscribe(OnContextUpdated).
                AddTo(LifeTime);
            return Unit.Default;
        }

        protected abstract IGameService CreateService();

        private void OnContextUpdated(IContext context)
        {
            if (isSharedSystem) {
                lock (this) {
                    _sharedService = _sharedService ?? 
                                     CreateService().AddTo(LifeTime);
                }
            }

            var service = isSharedSystem ? _sharedService : 
                CreateService().AddTo(LifeTime);
            service.Bind(context);
        }
    }
}
