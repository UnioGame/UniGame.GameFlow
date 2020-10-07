namespace UniGame.UniNodes.NodeSystem.Runtime.Core.Commands
{
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniRx;

    public interface IDataSourceCommand<TData> : ILifeTimeCompletionCommand
    {
        IReadOnlyReactiveProperty<TData> Value { get; }
    }
}