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
    /// <typeparam name="TServiceApi"></typeparam>
    [HideNode]
    public class GameServiceNode<TService, TServiceApi> : 
        ServiceNode<TServiceApi>
        where TServiceApi : class, IGameService
        where TService : class, TServiceApi, new()
    {
 
        protected override TServiceApi CreateService() => service ?? new TService();
        
    }
}
