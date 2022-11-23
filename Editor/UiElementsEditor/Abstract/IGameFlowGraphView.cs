namespace UniGame.GameFlowEditor.Editor
{
    using Runtime;
    using Core.Runtime;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Interfaces;

    public interface IGameFlowGraphView : ILifeTimeContext
    {
        IUniGraph ActiveGraph { get; }

    }
}