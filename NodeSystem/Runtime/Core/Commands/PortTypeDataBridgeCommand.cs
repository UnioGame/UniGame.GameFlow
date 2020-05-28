namespace UniGame.UniNodes.NodeSystem.Runtime.Core.Commands
{
    using System;
    using Interfaces;
    using Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;

    [Serializable]
    public class PortTypeDataBridgeCommand<TData> : IDataSourceCommand<TData>, IPortPair
    {
        protected readonly TData defaultValue;
        protected readonly bool distinctInput;
        
        protected IPortPair portPair;

        protected ReactiveProperty<TData> valueData = new ReactiveProperty<TData>();
        protected BoolReactiveProperty isFinalyze = new BoolReactiveProperty();

        public PortTypeDataBridgeCommand(
            IUniNode node,
            string portName,
            TData defaultValue,
            bool distinctInput = true)
        {
            this.defaultValue = defaultValue;
            this.valueData.Value = defaultValue;
            this.distinctInput = distinctInput;
            
            //create port pairs
            portPair = new ConnectedFormatedPairCommand(node, portName,false);
        }

        public bool IsComplete => isFinalyze.Value;

        public IReadOnlyReactiveProperty<TData> Value => valueData;

        public IPortValue InputPort => portPair.InputPort;

        public IPortValue OutputPort => portPair.OutputPort;
        
        public void Execute(ILifeTime lifeTime)
        {
            //reset local value
            lifeTime.AddCleanUpAction(CleanUpNode);
            
            var input = portPair.InputPort;
            var output = portPair.OutputPort;
            
            var valueObservable = input.Receive<TData>();
            
            var source = BindToDataSource(valueObservable);
            
            if (distinctInput) {
                source.Subscribe(x => valueData.Value = x).
                    AddTo(lifeTime);
            }
            else {
                source.Subscribe(valueData.SetValueAndForceNotify).
                    AddTo(lifeTime);
            }
            
            isFinalyze.
                Where(x => x).
                Do(x => output.Publish(valueData.Value)).
                Subscribe().
                AddTo(lifeTime);
        }

        protected virtual IObservable<TData> BindToDataSource(IObservable<TData> source)
        {
            return source;
        }
        
        private void CleanUpNode()
        {
            isFinalyze.Value = false;
            valueData.Value  = defaultValue;
        }

        public void Dispose()
        {
            CleanUpNode();
        }

        public void Complete()
        {
            isFinalyze.Value = true;
            isFinalyze.Value = false;
        }

    }
}
