namespace UniGame.UniNodes.GameFlow.Runtime.Nodes
{
    using Interfaces;
    using NodeSystem.Runtime.Attributes;

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
 
        protected override TService CreateService() => service ?? new TService();
        
    }
}
