using UnityEngine;

namespace HunterLands.Tests.Experiments.GraphExamples.States
{
    using UniGame.UniNodes.NodeSystem.Runtime.Attributes;
    using UniGame.UniNodes.NodeSystem.Runtime.Core;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGameFlow.Nodes.Runtime.States;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;

    [CreateNodeMenu("Examples/States/FlowTokenSource")]
    public class FlowTokenDemoSourceNode : UniNode
    {
        [Port(PortIO.Output)]
        public object tokenPort;

        private LifeTimeDefinition _tokenLifeTime;
        private LifeTimeDefinition TokenLifeTime => _tokenLifeTime = _tokenLifeTime ?? new LifeTimeDefinition();

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void FireToken()
        {
            var port        = GetPort(nameof(tokenPort));
            var outputValue = port.Value;
            var token       = new FlowAsyncStateToken().AddTo(TokenLifeTime);
            outputValue.Publish<IAsyncStateToken>(token);
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void TerminateToken()
        {
            TokenLifeTime.Release();
        }
        
    }
}
