﻿using UniGreenModules.UniNodeSystem.Runtime.Interfaces;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core.Commands
{
    using System;
    using UniRx;

    public class PortObjectDataBridgeCommand<TData>  : 
        PortTypeDataBridgeCommand<TData>
        where TData : class
    {
        private readonly bool ignoreEmpty;

        public PortObjectDataBridgeCommand(IUniNode node, string portName, TData defaultValue, bool distinctInput = true, bool ignoreEmpty = true) : 
            base(node, portName, defaultValue, distinctInput)
        {
            this.ignoreEmpty = ignoreEmpty;
        }

        protected override IObservable<TData> BindToDataSource(IObservable<TData> source)
        {
            return ignoreEmpty ? 
                source.Where(x => x != null) : 
                source;
        }
    }
}
