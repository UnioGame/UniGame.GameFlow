namespace UniGame.UniNodes.Examples.ContextNodes.SimpleServices.Runtime.Nodes
{
    using Context;
    using NodeSystem.Runtime.Core;
    using UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.Runtime.Attributes;
    using UniGreenModules.UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniNodes.Nodes.Runtime.Common;
    using UniRx;
    using UnityEngine;

    [CreateNodeMenu("Examples/DemoSystem/WaitGameStatus")]
    public class WaitGameStatus : InOutPortNode
    {
        [ReadOnlyValue]
        [SerializeField]
        private bool isGameReady = false;
        
        protected override void OnExecute()
        {
            
            PortPair.InputPort.Receive<IDemoGameStatus>().
                DistinctUntilChanged().
                Do(x => GameLog.Log("DATA IDemoGameStatus Received")).
                Select(x => x.IsGameReady).
                Switch().
                Do(x => isGameReady = x).
                Where(x => x).
                Do(x => PortPair.OutputPort.Publish(x)).
                Do(x => GameLog.Log("GAME INITIALIZED")).
                Subscribe().
                AddTo(LifeTime);
            
        }
    }
}
