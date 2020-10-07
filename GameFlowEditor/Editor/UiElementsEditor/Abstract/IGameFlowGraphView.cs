namespace UniGame.GameFlowEditor.Editor
{
    using Runtime;
    using UniModules.UniCore.Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniNodes.NodeSystem.Runtime.Core;
    using UniNodes.NodeSystem.Runtime.Interfaces;

    public interface IGameFlowGraphView : ILifeTimeContext
    {
        IUniGraph ActiveGraph { get; }

    }
}