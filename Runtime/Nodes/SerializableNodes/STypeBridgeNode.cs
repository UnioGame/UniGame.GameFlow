namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using System;
    using System.Collections.Generic;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Core.Commands;
    using UniModules.GameFlow.Runtime.Core.Nodes;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.Attributes;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces.Rx;
    using UniRx;
    using UnityEngine;

    [HideNode]
    [Serializable]
    public class STypeBridgeNode<TData> : SNode,
        IReadonlyRecycleReactiveProperty<TData>
    {
        protected const string portName = "context";
        
        #region inspector
        
        public bool distinctInput = true;

        [ReadOnlyValue]
        [SerializeField] protected TData defaultValue;

        #endregion

        protected IPortValue input;

        protected IPortValue output;
        
        protected IDataSourceCommand<TData> valueSource;

        protected IReadOnlyReactiveProperty<TData> valueData;

        #region public properties

        public IReadOnlyReactiveProperty<TData> Source => valueData;

        #endregion

        #region IReactiveProperty API

        public TData Value => valueData.Value;

        public bool HasValue => valueData.HasValue;
        
        public IDisposable Subscribe(IObserver<TData> observer) => valueData.Subscribe(observer);
        
        #endregion

        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);

            var command = new PortTypeDataBridgeCommand<TData>(this,portName,defaultValue,distinctInput);
            input = command.InputPort;
            output = command.OutputPort;
            
            valueSource = command;
            valueData   = valueSource.Value;

            nodeCommands.Add(valueSource);
        }

        public void Complete()
        {
            valueSource.Complete();
        }
    }
}