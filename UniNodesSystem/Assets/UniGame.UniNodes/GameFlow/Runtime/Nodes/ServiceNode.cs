namespace UniGame.UniNodes.GameFlow.Runtime.Nodes
{
    using Interfaces;
    using NodeSystem.Runtime.Attributes;
    using UniGreenModules.UniCore.Runtime.Attributes;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniNodes.Nodes.Runtime.Common;
    using UniRx;
    using UnityEngine;

    /// <summary>
    /// Base game service binder between Unity world and regular classes
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TServiceApi"></typeparam>
    [HideNode]
    public abstract class ServiceNode<TServiceApi> : 
        ContextNode
        where TServiceApi : IGameService
    {
        [SerializeField]
        protected TServiceApi service;

        #region inspector

        [Header("Service Status")]
        [ReadOnlyValue]
        [SerializeField]
        private bool isReady;
        
        #endregion
        
        public bool waitForServiceReady = true;

        protected abstract TServiceApi CreateService();

        protected override void OnInitialize()
        {
            base.OnInitialize();
            service = CreateService();
        }

        protected override void OnExecute()
        {
            Source.Where(x => x != null).
                Do(x => service.Bind(x,LifeTime)).
                CombineLatest(service.IsReady, (ctx, ready) => (ctx,ready)).
                Where(x => x.ready || !waitForServiceReady).
                Do(x => x.ctx.Publish(service)).
                Do(x => Finish()).
                Subscribe().
                AddTo(LifeTime);
        }

    }
}