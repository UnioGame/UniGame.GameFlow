namespace UniGame.UniNodes.Examples.ContextNodes.SimpleServices.Runtime
{
    using GameFlow.Runtime;
    using UniModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniRx;

    public class SimpleSystem4 : GameService
    {
        protected override IContext OnBind(IContext context, ILifeTime lifeTime)
        {
            isReady.Value = false;
            context.Publish(this);
            
            context.Receive<SimpleSystem3>().
                Where(x => x != null).
                Do(x => isReady.Value = true).
                Subscribe().
                AddTo(lifeTime);
            
            return context;
        }

    }
}
