namespace UniGame.UniNodes.GameFlow.Runtime.Commands
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.ProfilerTools;
    using Context.Runtime;
    using UniModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniModules.UniCore.Runtime.ProfilerTools;
    using Core.Runtime;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class RegisterDataSourcesCommand<TSource> : ILifeTimeCommand
        where TSource : AssetReference
    {
        private readonly UniTask<IContext> contextTask;
        private LoadAddressablesSourcesCommand<ScriptableObject,IAsyncContextDataSource> loadAssetCommand;
        
        public RegisterDataSourcesCommand(UniTask<IContext> contextTask,IReadOnlyList<TSource> resources)
        {
            this.contextTask = contextTask;
            this.loadAssetCommand = new LoadAddressablesSourcesCommand<ScriptableObject,IAsyncContextDataSource>(resources);
        }

        public async UniTask Execute(ILifeTime lifeTime)
        {
            var context = await contextTask;
            var resources = await loadAssetCommand.Execute(lifeTime);
            
            for (int i = 0; i < resources.Count; i++) {
                var resource = resources[i];
                if (resource == null) {
                    GameLog.LogError($"NULL asset loaded from {resource}");
                    return;
                }
                await resource.RegisterAsync(context);
            }

        }
    }
}
