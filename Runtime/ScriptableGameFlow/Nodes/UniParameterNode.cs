using System;
using GraphProcessor;
using UniGame.GameFlowEditor.Runtime;
using UniModules.GameFlow.Runtime.Interfaces;

namespace UniModules.UniGame.GameFlow.GameFlowEditor.Runtime.Nodes
{
    [NodeMenuItem("Hidden/Parameter")]
    [Serializable]
    public class UniParameterNode : UniBaseNode
    {
        private static string nodeStyle = "GameFlow/UCSS/ParameterStyle";
        
        public override string layoutStyle => nodeStyle;
    }
}