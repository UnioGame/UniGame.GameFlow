namespace UniGame.UniNodes.Examples.ContextNodes.SimpleServices.Runtime
{
    using Context;
    using GameFlow.Runtime;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.Attributes;
    using UniModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniModules.UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniRx;
    using UnityEngine;

    public class DemoSystemStatusService : GameService
    {
        [ReadOnlyValue]
        [SerializeField]
        public bool isGameReady = false;
        
        protected override IContext OnBind(IContext context, ILifeTime lifeTime)    
        {

            context.Receive<IDemoGameStatus>().
                Do(x => GameLog.Log($"DemoGameStatus has value {x.IsGameReady.HasValue} is Ready {x.IsGameReady.Value}")).
                Do(x => isGameReady = x.IsGameReady.Value).
                Where(x => x.IsGameReady.Value == false).
                Do(x => GameLog.Log("Mark Game Status as Ready")).
                Do(x => x.SetGameStatus(true)).
                Subscribe().
                AddTo(LifeTime);
                
            return context;
            
            context.Receive<SimpleSystem1>().
                CombineLatest(
                    context.Receive<SimpleSystem2>(), 
                    context.Receive<SimpleSystem3>(), 
                    context.Receive<SimpleSystem4>(),
                (x, y, z, k) => context).
                Do(x => GameLog.Log("Systems Messages received")).
                ContinueWith(x => x.Receive<IDemoGameStatus>()).
                Where(x => x.IsGameReady.Value == false).
                Do(x => GameLog.Log("Mark Game Status as Ready")).
                Do(x => x.SetGameStatus(true)).
                Subscribe().
                AddTo(lifeTime);
            
            return context;

        }

    }
}
