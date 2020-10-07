namespace UniGame.UniNodes.Examples.ContextNodes.SimpleServices.Runtime
{
    using GameFlow.Runtime;
    using UniModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniModules.UniCore.Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;

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
