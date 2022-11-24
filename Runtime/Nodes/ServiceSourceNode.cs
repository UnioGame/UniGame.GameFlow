using UniGame.UniNodes.Nodes.Runtime.Common;
using UniGame.AddressableTools.Runtime;
using UniGame.Context.Runtime;

namespace Game.Modules.Assets.UniGame.GameFlow.Runtime.Nodes
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniModules.GameFlow.Runtime.Attributes;
    using global::UniGame.Context.Runtime;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using global::UniGame.Core.Runtime;
    using UnityEngine.AddressableAssets;
    using Object = UnityEngine.Object;

    [HideNode]
    public class ServiceSourceNode<TSource> : ContextNode
        where TSource : Object, IAsyncContextDataSource
    {
        public AssetReferenceT<TSource> sourceAsset;

        public bool ownServiceLifeTime = true;
        
        protected sealed override async UniTask OnContextActivate(IContext context)
        {
            var serviceSource = await sourceAsset.LoadAssetTaskAsync(LifeTime);
            if (ownServiceLifeTime && serviceSource is IDisposable disposableService)
                disposableService.AddTo(LifeTime);
            
            var validation = await OnValidateSource(context, serviceSource)
                .AttachExternalCancellation(LifeTime.TokenSource);

            if (!validation)
            {
                CompleteProcessing(context);
                return;
            }

            context = await serviceSource.RegisterAsync(context);

            await OnSourceComplete(context, serviceSource);

            CompleteProcessing(context);
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