namespace UniGame.UniNodes.GameFlow.Runtime.Nodes
{
    using System.Collections.Generic;
    using NodeSystem.Runtime.Core;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime;
    using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniGreenModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniGreenModules.UniGame.Context.Runtime.Interfaces;
    using UniGreenModules.UniGame.SerializableContext.Runtime.Addressables;
    using UniNodes.Nodes.Runtime.Common;

    [CreateNodeMenu("GameSystem/Context Additional Sources", nodeName = "ContextRegisterValues")]
    public class ContextRegisterValuesNode : ContextNode
    {
        
        public List<AsyncContextDataSourceAssetReference> sources;

        protected override async void OnContextActivate(IContext context)
        {
            var results = ClassPool.Spawn<List<AsyncContextDataSource>>();

            await sources.LoadAssetsTaskAsync(results,LifeTime);

            foreach (var dataSource in results) {
                await dataSource.RegisterAsync(context);
            }
            
            results.Despawn();
            
            Finish();
        }

        protected sealed override void OnExecute()
        {
            foreach (var reference in sources) {
                LifeTime.AddDispose(reference);
            }
        }
    }
}
