namespace UniGame.UniNodes.GameFlow.Runtime.Commands
{
    using System;
    using UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniGreenModules.UniGame.SerializableContext.Runtime.Addressables;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
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

        public async void Execute(ILifeTime lifeTime)
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
