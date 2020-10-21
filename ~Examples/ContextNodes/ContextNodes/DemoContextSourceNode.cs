namespace UniGame.UniNodes.Examples.ContextNodes.ContextNodes
{
    using System;
    using NodeSystem.Runtime.Attributes;
    using NodeSystem.Runtime.Core;
    using NodeSystem.Runtime.ReactivePorts;
    using UniModules.UniContextData.Runtime.Entities;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UniRx;
    using UnityEngine;

    [CreateNodeMenu("Examples/ContextNodes/ContextSource","ContextSource")]
    public class DemoContextSourceNode : UniNode
    {
        [ReactivePort(PortIO.Output)]
        [SerializeField]
        public ContextReactivePort contextValue = new ContextReactivePort();
        
        public EntityContext context = new EntityContext();
        
        public BoolReactiveProperty clickToFireContext = new BoolReactiveProperty(false);

        public int intValue = 1;

        public float floatValule = 1f;

        protected override void OnInitialize()
        {
            contextValue.Initialize(this);
        }
        
        protected override void OnExecute()
        {
            clickToFireContext.
                Where(x => x).
                Subscribe(x => FireContext()).
                AddTo(LifeTime);
        }

        public void FireContext()
        {
            contextValue.Publish(context);
            context.Publish(intValue);
            context.Publish(floatValule);

            Observable.Timer(TimeSpan.FromSeconds(0.5)).
                Do(x => clickToFireContext.Value = false).
                Subscribe().
                AddTo(LifeTime);
            
        }
    }
}
