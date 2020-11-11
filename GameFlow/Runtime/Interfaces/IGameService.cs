namespace UniModules.UniGameFlow.GameFlow.Runtime.Interfaces
{
    using System;
    using global::UniGame.UniNodes.GameFlow.Runtime.Interfaces;
    using UniGame.Core.Runtime.Interfaces;

    public interface IGameService : 
        IDisposable, 
        ILifeTimeContext, 
        IReactiveStatus
    {
        void Complete();
    }
}
