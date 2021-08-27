namespace UniModules.UniGameFlow.Nodes.Runtime.States
{
    using System;
    using UniGame.Context.Runtime.Connections;
    using UniGame.Core.Runtime.Interfaces;

    public interface IStateToken :
        ILifeTimeContext,
        IDisposable
    {
        IContextConnection Context { get; }

        /// <summary>
        /// Try to launch target state for this execution token
        /// </summary>
        bool TakeOwnership(IStateCancellation state);

        /// <summary>
        /// stop all state "after"(exclude) target state
        /// </summary>
        bool StopAfter(IStateCancellation state);
        
        /// <summary>
        /// stop all state "since"(include) target state
        /// </summary>
        bool StopSince(IStateCancellation state);
    }
}