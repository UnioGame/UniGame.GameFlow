﻿namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using System;
    using System.Collections.Generic;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Core.Nodes;
    using UniModules.GameFlow.Runtime.Extensions;
    using UniModules.GameFlow.Runtime.Interfaces;
    using Core.Runtime;

    [HideNode]
    [Serializable]
    public class OutPortNode : SNode
    {
        public const string OutputPortName = "output";

        private IPortValue outputPortValue;
        
        public IPortValue OutputPort => outputPortValue;

        public override string GetStyle() => "GameFlow/UCSS/OutputPortNodeStyle";

        protected override void OnInitialize()
        {
            base.OnInitialize();
            outputPortValue = this.UpdatePortValue(OutputPortName, PortIO.Output);
        }
    }
}