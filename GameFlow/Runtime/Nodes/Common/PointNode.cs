using UniGame.UniNodes.NodeSystem.Runtime.Core.Nodes;

namespace UniModules.UniGame.GameFlow.GameFlow.Runtime.Nodes.Common
{
    using System;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Attributes;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Core;
    using UniCore.Runtime.Rx.Extensions;
    using UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UnityEngine;

    [Serializable]
    [CreateNodeMenu("Flow/Point")]
    [NodeInfo(
        nameof(PointNode),
        "flow",
        "serializable node for transfer data from input port to output")]
    public class PointNode : SNode
    {
        [Port(PortIO.Input)]
        private object input;

        [Port(PortIO.Output)]
        private object output;

        protected sealed override void OnExecute()
        {
            GetPortValue(nameof(input)).
                Bind(GetPortValue(nameof(output))).
                AddTo(LifeTime);
        }
    }
}
