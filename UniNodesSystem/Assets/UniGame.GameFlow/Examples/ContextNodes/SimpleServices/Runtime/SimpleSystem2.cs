namespace UniGame.UniNodes.Examples.ContextNodes.SimpleServices.Runtime
{
    using GameFlow.Runtime;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;

    public class SimpleSystem2 : GameService
    {
        protected override IContext OnBind(IContext context, ILifeTime lifeTime = null)
        {
            context.Publish(this);
            isReady.Value = true;
            return context;
        }    
    }
}
