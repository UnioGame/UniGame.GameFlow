namespace UniModules.UniGameFlow.GameFlow.Runtime.Systems
{
    using Interfaces;

    public class ContextServiceT<TService> : ContextService
        where TService :IGameService, new()
    {
        protected override IGameService CreateService() => new TService();
    }
}
