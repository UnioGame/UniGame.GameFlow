namespace UniGame.UniNodes.NodeSystem.Runtime.Interfaces
{
    using UniGreenModules.UniCore.Runtime.Interfaces;

    public interface IBrodcastConnector<TConnection> : 
        IContextWriter, 
        IConnector<TConnection>
    {
    }
}