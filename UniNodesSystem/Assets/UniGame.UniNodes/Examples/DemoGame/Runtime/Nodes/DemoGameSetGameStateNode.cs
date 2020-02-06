using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Nodes.Runtime.Nodes;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Examples.DemoGame.Runtime.Nodes
{
    using Models;
    using UniCore.Runtime.Rx.Extensions;
    using UniNodeSystem.Runtime.Core;
    using UniRx;

    [CreateNodeMenu("Examples/DemoGame/SetGameState")]
    public class DemoGameSetGameStateNode : ContextNode
    {
        public DemoGameState targetState;
        
        protected override void OnExecute()
        {
            Source.Select(x => x.Receive<DemoGameStatusData>()).Switch().
                Do(x => x.State.Value = targetState).
                Do(x => Finish()).
                Subscribe().
                AddTo(LifeTime);
        }
    }
}
