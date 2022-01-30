using UniGame.UniNodes.Nodes.Runtime.Common;
using UniModules.UniGame.SerializableContext.Runtime.Addressables;

namespace Game.Modules.Assets.UniGame.GameFlow.Runtime.Nodes
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniModules.GameFlow.Runtime.Attributes;
    using UniModules.UniContextData.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.Interfaces;
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