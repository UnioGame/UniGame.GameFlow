namespace UniGame.UniNodes.GameFlow.Runtime.Nodes
{
    using System;
    using Interfaces;
    using NodeSystem.Runtime.Attributes;
    using UniGreenModules.UniCore.Runtime.Attributes;
    using UniGreenModules.UniCore.Runtime.Interfaces;
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

        protected abstract TServiceApi CreateService(IContext context);

        private IDisposable _serviceDisposable;
        
        protected override void OnExecute()
        {
            Source.Where(x => x != null).
                Do(x=>service = CreateService(x)).
                Do(BindService).
                Subscribe().
                AddTo(LifeTime);
        }

        private void BindService(IContext context)
        {
            service.Bind(context, LifeTime);
            
            _serviceDisposable?.Dispose();

            _serviceDisposable = service.IsReady.
                Where(x => x || !waitForServiceReady).
                Do(_ => context.Publish(service)).
                Do(_ => Finish()).
                Subscribe().
                AddTo(LifeTime);
        }
    }
}