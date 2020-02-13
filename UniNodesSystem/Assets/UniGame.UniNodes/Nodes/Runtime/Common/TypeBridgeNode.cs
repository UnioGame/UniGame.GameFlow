namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Nodes.Runtime.Nodes
{
    using System;
    using System.Collections.Generic;
    using NodeSystem.Runtime.Attributes;
    using NodeSystem.Runtime.Nodes;
    using UniCore.Runtime.Attributes;
    using UniCore.Runtime.Interfaces;
    using UniCore.Runtime.Interfaces.Rx;
    using UniCore.Runtime.Rx.Extensions;
    using UniNodeSystem.Nodes.Commands;
    using UniNodeSystem.Runtime;
    using UniNodeSystem.Runtime.Interfaces;
    using UniRx;
    using UnityEngine;

    [HideNode]
    [Serializable]
    public class TypeBridgeNode<TData> : UniNode, 
        IUniInOutNode,
        IReadonlyRecycleReactiveProperty<TData>
    {
        private const string portName = "context";
        
        #region inspector

        [Header("take unique input")]
        public bool distinctInput = true;

        [Header("bind in/out")]
        public bool bindInOut = false;
        
        [ReadOnlyValue]
        public bool isFinalyze;
        
        [ReadOnlyValue]
        [SerializeField]
        protected TData value;

        #endregion

        #region private fields
        
        protected ReactiveProperty<TData> valueData;
        
        protected IPortValue input;
        
        protected IPortValue output;
        
        #endregion
        
        #region public properties

        public IObservable<TData> Source => valueData;

        public IPortValue Input => input;

        public IPortValue Output => output;
        
        #endregion

        #region IReactiveProperty API

        public TData Value => valueData.Value;

        public bool HasValue => valueData.HasValue;
        
        
        public IDisposable Subscribe(IObserver<TData> observer) => valueData.Subscribe(observer);

        
        #endregion
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            valueData = new ReactiveProperty<TData>();
            
            //reset local value
            LifeTime.AddCleanUpAction(CleanUpNode);
        }

        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);
            
            //create port pairs
            var portCommand = new ConnectedFormatedPairCommand(this,portName,bindInOut);
            nodeCommands.Add(portCommand);

            input = portCommand.InputPort;
            output = portCommand.OutputPort;

            
            var valueObservable = input.Receive<TData>();
            if (distinctInput) {
                valueObservable.
                    Subscribe(x => valueData.Value = x).
                    AddTo(LifeTime);
            }
            else {
                valueObservable.
                    Subscribe(valueData.SetValueAndForceNotify).
                    AddTo(LifeTime);
            }

            valueData.Subscribe(x => value = x).
                AddTo(LifeTime);
    
        }

        protected virtual void Finish()
        {
            isFinalyze = true;
            output.Publish(valueData.Value);
        }

        private void CleanUpNode()
        {
            isFinalyze = false;
            this.value = default;
        }

    }
}