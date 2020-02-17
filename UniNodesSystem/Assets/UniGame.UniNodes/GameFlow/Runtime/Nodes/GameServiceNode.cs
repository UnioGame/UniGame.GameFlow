namespace UniGreenModules.UniGameSystems.Runtime.Nodes
{
    using UniCore.Runtime.Attributes;
    using UniCore.Runtime.Interfaces;
    using UniCore.Runtime.ProfilerTools;
    using UniCore.Runtime.Rx.Extensions;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Nodes.Runtime.Nodes;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Attributes;
    using UniGameSystem.Runtime.Interfaces;
    using UniRx;
    using UnityEngine;

    [HideNode]
    public class GameServiceNode<TService> :
        GameServiceNode<TService, TService> where TService : IGameService, new() { }

    /// <summary>
    /// Base game service binder between Unity world and regular classes
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    [HideNode]
    public class GameServiceNode<TService,TServiceApi> : 
        ContextNode
        where TServiceApi : IGameService
        where TService : TServiceApi, new()
    {
        private TService service = new TService();

        #region inspector

        [Header("Service Status")]
        [ReadOnlyValue]
        [SerializeField]
        private bool isReady;
        
        #endregion
        
        public bool waitForServiceReady = true;

        protected override void OnExecute()
        {
            GameLog.LogMessage($"{Graph.ItemName}:{name}: Service {typeof(TService).Name}");

            Receive<IContext>().
                Do(x => service.Bind(x,LifeTime)).
                CombineLatest(service.IsReady, (ctx, ready) => (ctx,ready)).
                Where(x => x.ready || !waitForServiceReady).
                Do(x => x.ctx.Publish<TServiceApi>(service)).
                Subscribe().
                AddTo(LifeTime);

        }

    }
}
