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
    public class ServiceSourceNode<TSource> : ContextNode
        where TSource : Object, IAsyncContextDataSource
    {
        public AssetReferenceT<TSource> sourceAsset;

        protected sealed override async UniTask OnContextActivate(IContext context)
        {
            var serviceSource = await sourceAsset.LoadAssetTaskAsync(LifeTime);
            var validation = await OnValidateSource(context, serviceSource)
                .AttachExternalCancellation(LifeTime.TokenSource);

            if (!validation)
            {
                Complete();
                return;
            }

            context = await serviceSource.RegisterAsync(context);

            await OnSourceComplete(context, serviceSource);

            Complete();
        }

        protected virtual UniTask<bool> OnValidateSource(IContext context, TSource source)
        {
            return UniTask.FromResult(true);
        }

        protected virtual UniTask OnSourceComplete(IContext context, TSource source)
        {
            return UniTask.CompletedTask;
        }
    }
}