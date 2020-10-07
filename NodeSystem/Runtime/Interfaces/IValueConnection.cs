namespace UniGame.UniNodes.NodeSystem.Runtime.Interfaces
{
    using Core;
    using UniModules.UniCore.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime.Interfaces;

    public interface IValueConnection<TValue>  : IDataValue<TValue>,IPoolable
    {
        
        string Id { get; }

        PortIO Direction { get; }
        
    }
}
