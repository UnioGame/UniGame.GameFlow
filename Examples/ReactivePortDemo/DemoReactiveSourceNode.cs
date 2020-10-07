namespace UniGame.UniNodes.Examples.ReactivePortDemo
{
    using System;
    using NodeSystem.Runtime.Attributes;
    using NodeSystem.Runtime.Core;
    using NodeSystem.Runtime.ReactivePorts;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UniRx;
    using UnityEngine;

    [CreateNodeMenu("Examples/ReactivePortDemo/ReactiveSource","ReactiveSource")]
    public class DemoReactiveSourceNode : UniNode
    {
        
        #region inspector

        [SerializeField]
        private float delay = 2f;
        
        [SerializeField]
        private int value = 1;
        
        #endregion
        
        /// <summary>
        /// bind local to port
        /// </summary>
        [ReactivePort(PortIO.Output)]
        public IntReactivePort IntOut = new IntReactivePort();

        /// <summary>
        /// custom output port name
        /// </summary>
        [ReactivePort(fieldName = "customOutput",direction = PortIO.Output)]
        public IntReactivePort outValue = new IntReactivePort();
        
        protected override void OnInitialize()
        {
            IntOut.Initialize(this);
            outValue.Initialize(this);
        }

        protected override void OnExecute()
        {
            Observable.Interval(TimeSpan.FromSeconds(delay)).
                Do(x => IntOut.Publish(++value)).
                Subscribe().
                AddTo(LifeTime);
        }

    }
}
