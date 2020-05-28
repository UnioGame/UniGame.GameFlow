namespace UniGame.UniNodes.Examples.ContextNodes.SimpleServices.Runtime.Context
{
    using UniRx;

    public interface IDemoGameStatus : IImmutableDemoGameStatus
    {
        IReadOnlyReactiveProperty<bool> SetGameStatus(bool isReady);

    }
}