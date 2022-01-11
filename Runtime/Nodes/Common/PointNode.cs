using UniModules.GameFlow.Runtime.Core.Nodes;

namespace UniModules.UniGame.GameFlow.GameFlow.Runtime.Nodes.Common
{
    using System;
    using Cysharp.Threading.Tasks;
    using global::UniModules.GameFlow.Runtime.Attributes;
    using global::UniModules.GameFlow.Runtime.Core;
    using UniCore.Runtime.Rx.Extensions;
    using UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UnityEngine;

    [Serializable]
    [CreateNodeMenu("Common/Point")]
    [NodeInfo(nameof(PointNode), "flow", "serializable node for transfer data from input port to output")]
    public class PointNode : SNode
    {
        [Port(PortIO.Input)]
        private object input;

        [Port(PortIO.Output)]
        private object output;

        protected sealed override UniTask OnExecute()
        {
            GetPortValue(nameof(input)).
                Broadcast(GetPortValue(nameof(output))).
                AddTo(LifeTime);
            
            return UniTask.CompletedTask;
        }
    }
}
