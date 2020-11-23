namespace UniModules.UniGameFlow.Nodes.Runtime.States
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniGame.Context.SerializableContext.Runtime.Abstract;
    using UniGame.Core.Runtime.Interfaces;

    public interface IAsyncStateToken :
        ILifeTimeContext,
        IDisposable
    {
        IContext      Context { get; }
        int           Id      { get; }
        
        /// <summary>
        /// Try to launch target state for this execution token
        /// </summary>
        UniTask<bool> TakeOwnership(IAsyncContextState state);

        /// <summary>
        /// stop all state "after"(exclude) target state
        /// </summary>
        UniTask<bool> StopAfter(IAsyncContextState state);
        
        /// <summary>
        /// stop all state "since"(include) target state
        /// </summary>
        UniTask<bool> StopSince(IAsyncContextState state);
    }
}