namespace UniModules.GameFlow.Runtime.Interfaces
{
    using UniModules.UniGame.Core.Runtime.Interfaces;

    public interface IBroadcastConnector<TConnection> : 
        IContextWriter, 
        IBroadcaster<TConnection>
    {
    }
}