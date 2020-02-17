namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Nodes.Runtime.Nodes
{
    using System;
    using System.Collections.Generic;
    using NodeSystem.Runtime.Attributes;
    using NodeSystem.Runtime.Core.Commands;
    using NodeSystem.Runtime.Nodes;
    using UniCore.Runtime.Attributes;
    using UniCore.Runtime.Interfaces;
    using UniCore.Runtime.Interfaces.Rx;
    using UniCore.Runtime.Rx.Extensions;
    using UniNodeSystem.Nodes.Commands;
    using UniNodeSystem.Runtime.Interfaces;
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