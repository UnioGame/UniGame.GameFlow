namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Nodes.Runtime.Nodes
{
    using System;
    using System.Collections.Generic;
    using NodeSystem.Runtime.Attributes;
    using NodeSystem.Runtime.Nodes;
    using UniCore.Runtime.Attributes;
    using UniCore.Runtime.Interfaces;
    using UniCore.Runtime.Rx.Extensions;
    using UniNodeSystem.Nodes.Commands;
    using UniNodeSystem.Runtime;
    using UniNodeSystem.Runtime.Interfaces;
    using UniRx;
    using UnityEngine;

    [HideNode]
    [Serializable]
    public class TypeBridgeNode<TData> : UniNode, IUniInOutNode
    {
        private const string portName = "context";
        
        #region inspector

        [Header("take unique input")]
        public bool distinctInput = true;

        [Header("bind in/out")]
        public bool bindInOut = false;
        
        [ReadOnlyValue]
        public BoolReactiveProperty isComplete;
        
        [ReadOnlyValue]
        [SerializeField]
        protected TData value;

        #endregion

        private IPortValue input;
        
        private IPortValue output;
        
        #region public properties
        
        
        public IObservable<TData> Source { get; protected set; }

        public IPortValue Input => input;

        public IPortValue Output => output;
        
        #endregion

        protected override void OnInitialize()
        {
            base.OnInitialize();
            //reset local value
            LifeTime.AddCleanUpAction(() => this.value = default);
        }

        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);
            
            //create port pairs
            var portCommand = new ConnectedFormatedPairCommand();
            portCommand.Initialize(this,portName,bindInOut);
            nodeCommands.Add(portCommand);

            input = portCommand.InputPort;
            output = portCommand.OutputPort;

            var dataCommand = new PortDataBridgeActionCommand<TData>(input, distinctInput);
            commands.Add(dataCommand);
            
            //subscribe to TData stream
            var valueObservable = dataCommand.Source;
            //set local value at any changes
            valueObservable.Do(x => value = x);
            
            Source = valueObservable;

        }

        protected virtual void Finish()
        {
            output.Publish(value);
        }
        
    }
}