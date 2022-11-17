namespace UniGame.UniNodes.GameFlow.Runtime.Nodes
{
    using UniModules.GameFlow.Runtime.Attributes;
    using UniGame.GameFlow.Runtime.Interfaces;
    
    /// <summary>
    /// Base game service binder between Unity world and regular classes
    /// </summary>
    /// <typeparam name="TServiceApi"></typeparam>
    [HideNode]
    public abstract class GameServiceNode<TServiceApi> : 
        ServiceNode<TServiceApi>
        where TServiceApi : class, IGameService
    {
    }
}
