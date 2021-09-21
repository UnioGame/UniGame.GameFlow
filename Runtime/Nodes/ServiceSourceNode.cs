using UniGame.UniNodes.Nodes.Runtime.Common;

namespace Game.Modules.Assets.UniGame.GameFlow.Runtime.Nodes
{
    using Cysharp.Threading.Tasks;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniModules.UniContextData.Runtime.Interfaces;
    using UniModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    [HideNode]
    public class ServiceSourceNode<TService> : ContextNode
        where TService : Object, IAsyncContextDataSource
    {
        public AssetReferenceT<TService> sourceAsset;

        protected sealed override async UniTask OnContextActivate(IContext context)
        {
            var source = await sourceAsset.LoadAssetTaskAsync(LifeTime);
            context = await source.RegisterAsync(context);

            await OnSourceComplete(context);
            
            Complete();
        }

        protected virtual UniTask OnSourceComplete(IContext context)
        {
            return UniTask.CompletedTask;
        }
    }
}