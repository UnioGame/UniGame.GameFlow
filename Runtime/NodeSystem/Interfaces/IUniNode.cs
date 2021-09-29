namespace UniModules.GameFlow.Runtime.Interfaces
{
    using UniCore.Runtime.ObjectPool.Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;

    public interface IUniNode : 
        INode,
        IAsyncCommand,
        IEndPoint,
        ILifeTimeContext,
        IActiveStatus,
        IPoolable
    {

    }
}