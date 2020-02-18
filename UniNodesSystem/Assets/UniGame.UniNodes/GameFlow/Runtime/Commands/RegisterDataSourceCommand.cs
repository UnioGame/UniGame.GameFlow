namespace UniGame.UniNodes.GameFlow.Runtime.Commands
{
    using System;
    using UniGreenModules.UniContextData.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniRx.Async;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using Object = System.Object;

    [Serializable]
    public class RegisterDataSourceCommand : ILifeTimeCommand
    {
        private readonly UniTask<IContext> contextTask;
        private readonly AssetReference resource;

        public Object asset;
        public IAsyncContextDataSource dataSource;

        public RegisterDataSourceCommand(UniTask<IContext> contextTask,AssetReference resource)
        {
            this.contextTask = contextTask;
            this.resource = resource;
        }

        public async void Execute(ILifeTime lifeTime)
        {
            asset = await resource.LoadAssetTaskAsync<ScriptableObject>();
            dataSource = asset as IAsyncContextDataSource;
            if (asset == null) {
                GameLog.LogError($"NULL asset loaded from {resource}");
                return;
            }
            
            await dataSource.RegisterAsync(await contextTask);
            
        }
    }
}
