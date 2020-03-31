namespace UniGame.GameFlowEditor.Editor
{
    using Runtime;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniNodes.NodeSystem.Runtime.Core;
    using UniNodes.NodeSystem.Runtime.Interfaces;

    public interface IGameFlowGraphView : ILifeTimeContext
    {
        IUniGraph ActiveGraph { get; }

    }
}