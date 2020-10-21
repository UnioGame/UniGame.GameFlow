namespace UniGame.UniNodes.Examples.ContextNodes.SimpleServices.Runtime.Context
{
    using UniRx;

    public interface IImmutableDemoGameStatus
    {
        IReadOnlyReactiveProperty<bool> IsGameReady { get; }
    }
}