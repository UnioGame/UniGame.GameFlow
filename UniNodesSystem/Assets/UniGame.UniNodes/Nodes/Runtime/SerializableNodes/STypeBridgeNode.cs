namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using System;
    using System.Collections.Generic;
    using NodeSystem.Runtime.Attributes;
    using NodeSystem.Runtime.Core;
    using NodeSystem.Runtime.Core.Commands;
    using NodeSystem.Runtime.Core.Nodes;
    using NodeSystem.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.Attributes;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces.Rx;
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

        #region constructor

        public STypeBridgeNode(){}

        public STypeBridgeNode(
            int id,
            string name,
            NodePortDictionary ports) : base(id, name, ports){}

        #endregion
        
        
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

        public void Finish()
        {
            valueSource.Complete();
        }

    }
}