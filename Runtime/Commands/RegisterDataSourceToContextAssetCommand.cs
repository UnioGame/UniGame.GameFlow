﻿using UniGame.AddressableTools.Runtime;
using UniGame.Context.Runtime;

namespace UniGame.UniNodes.GameFlow.Runtime.Commands
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniModules.UniCore.Runtime.ProfilerTools;
    using Context.Runtime;
    using Core.Runtime;

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
            
            if (asset is not IAsyncDataSource asyncSource) return;
            
            await asyncSource.RegisterAsync(context.Value);
        }
    }
}
