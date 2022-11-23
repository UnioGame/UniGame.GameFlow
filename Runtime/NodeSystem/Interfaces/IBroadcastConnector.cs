namespace UniModules.GameFlow.Runtime.Interfaces
{
    using global::UniGame.Core.Runtime;

    public interface IBroadcastConnector<TConnection> : 
        IContextWriter, 
        IBroadcaster<TConnection>
    {
    }
}