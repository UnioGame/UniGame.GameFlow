namespace UniGame.UniNodes.NodeSystem.Runtime.Interfaces
{
    using UniModules.UniGame.Core.Runtime.Interfaces;

    public interface IBrodcastConnector<TConnection> : 
        IContextWriter, 
        IBroadcaster<TConnection>
    {
    }
}