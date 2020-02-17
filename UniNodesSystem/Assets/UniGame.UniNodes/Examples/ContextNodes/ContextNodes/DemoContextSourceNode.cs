namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Examples.ContextNodes.ContextNodes
{
    using System;
    using NodeSystem.Runtime.Attributes;
    using NodeSystem.Runtime.Nodes;
    using NodeSystem.Runtime.ReactivePorts;
    using UniContextData.Runtime.Entities;
    using UniCore.Runtime.Rx.Extensions;
    using UniNodeSystem.Runtime.Core;
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
