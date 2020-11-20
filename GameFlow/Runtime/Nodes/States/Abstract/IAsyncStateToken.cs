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
        UniTask<bool> TakeOwnership(IAsyncContextState state);

        UniTask<bool> StopAfter(IAsyncContextState node);
        
        UniTask<bool> StopSince(IAsyncContextState node);
    }
}