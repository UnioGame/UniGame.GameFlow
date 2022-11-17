namespace UniGame.UniNodes.Nodes.Runtime.Common
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Core.Nodes;
    using UniModules.GameFlow.Runtime.Extensions;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.Attributes;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces.Rx;
    using UniModules.UniGame.Core.Runtime.Rx;
    using UniRx;
    using UnityEngine;

    [HideNode]
    [Serializable]
    public class STypeBridgeNode<TData> : 
        SNode,
        IReadonlyRecycleReactiveProperty<TData>
    {
        protected const string portName = "context";
        
        #region inspector
        
        public bool distinctInput = true;
        public bool completeOnce  = false;
        
        [ReadOnlyValue]
        [SerializeField]
        protected TData editorValue;

        #endregion
        
        private RecycleReactiveProperty<TData> _valueData;
        private RecycleReactiveProperty<bool>  _isReady;

        public IPortValue input;
        public IPortValue output;

        #region public properties

        public IPortValue Output => output;

        public IPortValue Input => input;
        
        public IReadOnlyReactiveProperty<TData> Source => _valueData;

        #endregion

        #region IReactiveProperty API

        public TData Value => _valueData.Value;

        public bool HasValue => _valueData.HasValue;
        
        public IDisposable Subscribe(IObserver<TData> observer) => _valueData.Subscribe(observer);
        
        #endregion

        protected sealed override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);

            _valueData = new RecycleReactiveProperty<TData>().AddTo(LifeTime);
            _isReady   = new RecycleReactiveProperty<bool>().AddTo(LifeTime);
            
            var inputName   = portName.GetFormatedPortName(PortIO.Input);
            var outputName  = portName.GetFormatedPortName(PortIO.Output);
            var bridgePorts = this.CreatePortPair(inputName, outputName, false);

            input  = bridgePorts.inputValue;
            output = bridgePorts.outputValue;
        }

        protected sealed override UniTask OnExecute()
        {
            _valueData
                .Subscribe(x => OnValueUpdate(x)
                    .AttachExternalCancellation(LifeTime.TokenSource)
                    .Forget())
                .AddTo(LifeTime);

            _isReady.Where(x => x)
                .CombineLatest(_valueData, (x, value) => value)
                .Subscribe(output.Publish)
                .AddTo(LifeTime);
            
            //reset local value
            var valueObservable = input.Receive<TData>();

            if (distinctInput)
                valueObservable.Subscribe(x => _valueData.Value = x).AddTo(LifeTime);
            else
                valueObservable.Subscribe(_valueData.SetValueForce).AddTo(LifeTime);

            return UniTask.CompletedTask;
        }

        public void Complete()
        {
            _isReady.Value = true;
            if (completeOnce) return;
            _isReady.Value = false;
        }

        protected virtual UniTask OnValueUpdate(TData value)
        {
            return UniTask.CompletedTask;
        }
    }
}