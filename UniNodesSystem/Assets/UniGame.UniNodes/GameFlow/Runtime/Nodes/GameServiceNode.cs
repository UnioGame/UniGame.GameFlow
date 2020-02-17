namespace UniGreenModules.UniGameSystems.Runtime.Nodes
{
    using UniCore.Runtime.Attributes;
    using UniCore.Runtime.Interfaces;
    using UniCore.Runtime.ProfilerTools;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Attributes;
    using UniGameSystem.Runtime.Interfaces;
    using UnityEngine;

    [HideNode]
    public abstract class GameServiceNode<TService> :
        GameServiceNode<TService, TService> where TService : class, IGameService, new() { }

    /// <summary>
    /// Base game service binder between Unity world and regular classes
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    [HideNode]
    public class GameServiceNode<TService,TServiceApi> : 
        ServiceNode<TService,TServiceApi>
        where TServiceApi : IGameService
        where TService : class, TServiceApi, new()
    {
        #region inspector

        [Header("Service Status")]
        [ReadOnlyValue]
        [SerializeField]
        private bool isReady;
        
        #endregion
 
        protected override TService CreateService() => service ?? new TService();
        
    }
}
