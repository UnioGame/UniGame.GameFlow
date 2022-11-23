namespace UniModules.GameFlow.Runtime.Core.Commands
{
    using System;
    using Cysharp.Threading.Tasks;
    using Interfaces;
    using Runtime.Interfaces;
    using UniGame.Core.Runtime.Rx;
    using UniModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using global::UniGame.Core.Runtime;
    using UniRx;

    [Serializable]
    public class PortTypeDataBridgeCommand<TData> : IDataSourceCommand<TData>, IPortPair
    {
        private readonly TData _defaultValue;
        private readonly bool  _distinctInput;

        private IPortPair                      _portPair;
        private INode                          _node;
        private RecycleReactiveProperty<TData> _valueData  = new RecycleReactiveProperty<TData>();
        private RecycleReactiveProperty<bool>  _isFinalyze = new RecycleReactiveProperty<bool>();

        public PortTypeDataBridgeCommand(
            IUniNode node,
            string portName,
            TData defaultValue,
            bool distinctInput = true)
        {
            _node                 = node;
            _defaultValue    = defaultValue;
            _valueData.Value = defaultValue;
            _distinctInput   = distinctInput;

            //create port pairs
            _portPair = new ConnectedFormatedPairCommand(node, portName, false);
        }

        public bool IsComplete => _isFinalyze.Value;

        public IReadOnlyReactiveProperty<TData> Value => _valueData;

        public IPortValue InputPort => _portPair.InputPort;

        public IPortValue OutputPort => _portPair.OutputPort;

        public UniTask Execute(ILifeTime lifeTime)
        {
            //reset local value
            lifeTime.AddCleanUpAction(CleanUpNode);

            var input  = _portPair.InputPort;
            var output = _portPair.OutputPort;

            var valueObservable = input.Receive<TData>();
            var source          = BindToDataSource(valueObservable);

            if (_distinctInput)
            {
                source.Subscribe(x => _valueData.Value = x).AddTo(lifeTime);
            }
            else
            {
                source.Subscribe(_valueData.SetValueForce).AddTo(lifeTime);
            }

            _isFinalyze.Where(x => x)
                .Do(x => output.Publish(_valueData.Value))
                .Subscribe()
                .AddTo(lifeTime);

            return UniTask.CompletedTask;
        }

        protected virtual IObservable<TData> BindToDataSource(IObservable<TData> source)
        {
            return source;
        }

        private void CleanUpNode()
        {
            _isFinalyze.Value = false;
            _valueData.Value  = _defaultValue;
        }

        public void Dispose() => CleanUpNode();

        public void Complete()
        {
            _isFinalyze.Value = true;
            _isFinalyze.Value = false;
        }
    }
}