using UniGame.AddressableTools.Runtime;
using UniGame.Core.Runtime.Extension;

namespace UniGame.UniNodes.GameFlow.Runtime.Nodes
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniGame.Runtime.ObjectPool;
    using UniGame.Runtime.ObjectPool.Extensions;
    using Context.Runtime;
    using Core.Runtime;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UniNodes.Nodes.Runtime.Common;

    [CreateNodeMenu("Common/Sources/Context Additional Sources", nodeName = "ContextRegisterValues")]
    public class ContextRegisterValuesNode : ContextNode
    {
        public List<AssetReferenceDataSource> sources;

        protected override async UniTask OnContextActivate(IContext context)
        {
            var results = ClassPool.Spawn<List<AsyncSource>>();

            await sources.LoadAssetsTaskAsync(results,LifeTime);

            foreach (var dataSource in results) {
                await dataSource.ToSharedInstance(LifeTime)
                    .RegisterAsync(context);
            }
            
            results.Despawn();
            
            CompleteProcessing(context);
        }
    }
}
