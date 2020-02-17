namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core.Commands
{
    using UniGreenModules.UniGame.Core.Runtime.Interfaces;
    using UniRx;

    public interface IDataSourceCommand<TData> : ILifeTimeCompletionCommand
    {
        IReadOnlyReactiveProperty<TData> Value { get; }
    }
}