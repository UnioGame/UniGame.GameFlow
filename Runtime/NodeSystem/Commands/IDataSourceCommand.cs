namespace UniModules.GameFlow.Runtime.Core.Commands
{
    using global::UniGame.Core.Runtime;
    using UniRx;

    public interface IDataSourceCommand<TData> : ILifeTimeCompletionCommand
    {
        IReadOnlyReactiveProperty<TData> Value { get; }
    }
}