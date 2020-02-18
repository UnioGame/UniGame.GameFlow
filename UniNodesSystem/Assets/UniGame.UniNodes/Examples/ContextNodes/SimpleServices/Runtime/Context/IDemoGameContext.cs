namespace Examples.ContextNodes.SimpleServices.Runtime.Context
{
    using UniGame.UniNodes.Examples.ContextNodes.SimpleServices.Runtime.Context;

    public interface IDemoGameContext
    {
        IDemoGameStatus GameStatus { get; }
    }
}