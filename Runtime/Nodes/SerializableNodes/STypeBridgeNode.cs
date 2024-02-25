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
    using UniCore.Runtime.Attributes;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using Core.Runtime;
    using Core.Runtime.Rx;
    using UniModules.UniGame.Core.Runtime.Rx;
    using UniRx;
    using UnityEngine;

    [HideNode]
    [Serializable]
    public class STypeBridgeNode<TData> : 
        SNode,
        IReadonlyReactiveValue<TData>
    {
        protected const string portName = "context";
        
        #region inspector
        
        public bool distinctInput = true;
        public bool completeOnce  = false;
        
        [ReadOnlyValue]
        [SerializeField]
        protected TData editorValue;

        #endregion
        
        private ReactiveValue<TData> _valueData;
        private ReactiveValue<bool>  _isReady;
        private bool _isFinished = false;

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

            _valueData = new ReactiveValue<TData>().AddTo(LifeTime);
            _isReady   = new ReactiveValue<bool>().AddTo(LifeTime);
            
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
                    .AttachExternalCancellation(LifeTime.Token)
                    .Forget())
                .AddTo(LifeTime);
            
            //
            // _isReady.Where(x => x)
            //     .CombineLatest(_valueData, (x, value) => value)
            //     .Subscribe(output.Publish)
            //     .AddTo(LifeTime);
            
            //reset local value
            var valueObservable = input.Receive<TData>();

            if (distinctInput)
                valueObservable.Subscribe(x => _valueData.Value = x).AddTo(LifeTime);
            else
                valueObservable.Subscribe(_valueData.SetValueForce).AddTo(LifeTime);

            return UniTask.CompletedTask;
        }

        public void CompleteProcessing(TData data)
        {
            if (_isFinished) return;
            _isFinished = completeOnce;
            output.Publish(data);
        }

        protected virtual UniTask OnValueUpdate(TData value)
        {
            return UniTask.CompletedTask;
        }
    }
}