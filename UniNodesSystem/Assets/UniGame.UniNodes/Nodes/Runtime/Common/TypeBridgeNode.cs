namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using System;
    using System.Collections.Generic;
    using NodeSystem.Runtime.Attributes;
    using NodeSystem.Runtime.Core;
    using NodeSystem.Runtime.Core.Commands;
    using UniGreenModules.UniCore.Runtime.Attributes;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces.Rx;
    using UniRx;
    using UnityEngine;

    [HideNode]
    [Serializable]
    public class TypeBridgeNode<TData> : UniNode,
        IReadonlyRecycleReactiveProperty<TData>
    {
        protected const string portName = "context";
        
        #region inspector
        
        public bool distinctInput = true;

        [ReadOnlyValue]
        [SerializeField] protected TData defaultValue;

        #endregion
        
        protected IDataSourceCommand<TData> valueSource;

        protected IReadOnlyReactiveProperty<TData> valueData;

        #region private fields

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

            valueSource = new PortTypeDataBridgeCommand<TData>(this,portName,defaultValue,distinctInput);
            valueData   = valueSource.Value;

            nodeCommands.Add(valueSource);
        }

        protected void Finish()
        {
            valueSource.Complete();
        }

    }
}