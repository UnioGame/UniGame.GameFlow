﻿using UniGame.GameFlowEditor.Runtime;
using UniModules.UniGame.GameFlow.GameFlowEditor.Editor.Attributes;
using UniModules.UniGame.GameFlow.GameFlowEditor.Runtime.Nodes;

namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;

    [Serializable]
    [CreateNodeMenu("Parameters/GraphContextData")]
    [NodeAsset(typeof(ParameterView))]
    [NodeInfo(nameof(GraphContextOutputNode), "output", "pass graph context as parameter")]
    public class GraphContextOutputNode : GraphParameterDataNode, IGraphParameter
    {
        public const string GraphContextOutputName = "Graph Context";
        
        public sealed override string ItemName => GraphContextOutputName;

        protected override UniTask OnExecute()
        {
            var output = GetPortValue(OutputPortName);
            output.Publish(Context);
            return UniTask.CompletedTask;
        }
    }

    [Serializable]
    public class GraphContextParameter : UniExposedParameter<GraphContextOutputNode> { }
}