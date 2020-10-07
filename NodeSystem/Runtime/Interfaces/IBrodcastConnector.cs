namespace UniGame.UniNodes.NodeSystem.Runtime.Interfaces
{
    using UniModules.UniCore.Runtime.Interfaces;

    public interface IBrodcastConnector<TConnection> : 
        IContextWriter, 
        IConnector<TConnection>
    {
    }
}