namespace UniGame.UniNodes.GameFlow.Runtime.Nodes
{
    using System.Collections.Generic;
    using NodeSystem.Runtime.Core;
    using UniModules.UniCore.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniModules.UniGame.SerializableContext.Runtime.Addressables;
    using UniModules.UniGame.Context.Runtime.Abstract;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UniNodes.Nodes.Runtime.Common;

    [CreateNodeMenu("GameSystem/Context Additional Sources", nodeName = "ContextRegisterValues")]
    public class ContextRegisterValuesNode : ContextNode
    {
        
        public List<AssetReferenceDataSource> sources;

        protected override async void OnContextActivate(IContext context)
        {
            var results = ClassPool.Spawn<List<AsyncContextDataSource>>();

            await sources.LoadAssetsTaskAsync(results,LifeTime);

            foreach (var dataSource in results) {
                await dataSource.RegisterAsync(context);
            }
            
            results.Despawn();
            
            Complete();
        }

        protected sealed override void OnExecute()
        {
            foreach (var reference in sources) {
                LifeTime.AddDispose(reference);
            }
        }
    }
}
