namespace UniModules.GameFlow.Runtime.Interfaces
{
    using global::UniGame.Core.Runtime.ObjectPool;
    using global::UniGame.Core.Runtime;

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