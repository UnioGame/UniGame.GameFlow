namespace UniModules.GameFlow.Runtime.Interfaces
{
    using Core;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;

    public interface IValueConnection<TValue>  : IDataValue<TValue>,IPoolable
    {
        
        string Id { get; }

        PortIO Direction { get; }
        
    }
}
