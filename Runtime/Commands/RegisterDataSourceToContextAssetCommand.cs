using UniModules.UniGame.CoreModules.UniGame.AddressableTools.Runtime.Extensions;

namespace UniGame.UniNodes.GameFlow.Runtime.Commands
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniModules.UniCore.Runtime.ProfilerTools;
    using UniModules.UniGame.SerializableContext.Runtime.Addressables;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGame.SerializableContext.Runtime.Addressables;
    using UniModules.UniGame.SerializableContext.Runtime.AssetTypes;

    [Serializable]
    public class RegisterDataSourceToContextAssetCommand : ILifeTimeCommand
    {
        private readonly ContextAssetReference contextResource;
        private readonly AssetReferenceDataSource resource;

        public RegisterDataSourceToContextAssetCommand(ContextAssetReference contextResource,AssetReferenceDataSource resource)
        {
            this.contextResource = contextResource;
            this.resource = resource;
        }

        public async UniTask Execute(ILifeTime lifeTime)
        {
            var context = await contextResource.LoadAssetTaskAsync<ContextAsset>(lifeTime);
            var asset = await resource.LoadAssetTaskAsync(lifeTime);
            
            if (asset == null || !context) {
                GameLog.LogError($"NULL asset loaded from {resource} context {contextResource}");
                return;    
            }
            
            await asset.RegisterAsync(context.Value);
        }
    }
}
