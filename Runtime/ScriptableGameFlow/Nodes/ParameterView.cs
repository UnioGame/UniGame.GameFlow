using System;
using GraphProcessor;
using UniGame.GameFlowEditor.Runtime;

namespace UniModules.UniGame.GameFlow.GameFlowEditor.Runtime.Nodes
{
    [NodeMenuItem("Hidden/Parameter")]
    [Serializable]
    public class ParameterView : UniBaseNode
    {
        private static string nodeStyle = "GameFlow/UCSS/ParameterStyle";
        
        public override string layoutStyle => nodeStyle;
        
    }
}