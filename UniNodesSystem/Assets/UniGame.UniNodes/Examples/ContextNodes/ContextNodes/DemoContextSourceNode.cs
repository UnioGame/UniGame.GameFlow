namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Examples.ContextNodes.ContextNodes
{
    using NodeSystem.Runtime.Attributes;
    using NodeSystem.Runtime.Nodes;
    using NodeSystem.Runtime.ReactivePorts;
    using UniContextData.Runtime.Entities;
    using UniCore.Runtime.Rx.Extensions;
    using UniNodeSystem.Runtime.Core;
    using UniRx;

    [CreateNodeMenu("Examples/ContextNodes/ContextSource","ContextSource")]
    public class DemoContextSourceNode : UniNode
    {
        [ReactivePort(PortIO.Output)]
        public ContextReactivePort contextValue = new ContextReactivePort();
        
        public EntityContext context = new EntityContext();
        
        public BoolReactiveProperty onFireContext = new BoolReactiveProperty(false);

        public int intValue = 1;

        public float floatValule = 1f;
        
        protected override void OnExecute()
        {
            onFireContext.Subscribe(x => FireContext()).AddTo(LifeTime);
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button("Set Context")]
#endif
        public void FireContext()
        {
            contextValue.SetValue(context);
            context.Publish(intValue);
            context.Publish(floatValule);
        }
    }
}
