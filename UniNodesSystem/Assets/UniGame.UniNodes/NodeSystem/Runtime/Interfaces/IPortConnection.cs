namespace UniGame.UniNodes.NodeSystem.Runtime.Interfaces
{
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime.Interfaces;

    public interface IPortConnection : IContextWriter, IConnector<ITypeData>, IPoolable
    {
    }
}