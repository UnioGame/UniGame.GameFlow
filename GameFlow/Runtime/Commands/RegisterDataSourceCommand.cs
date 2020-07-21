namespace UniGame.UniNodes.GameFlow.Runtime.Commands
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniContextData.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using Object = System.Object;

    [Serializable]
    public class RegisterDataSourceCommand : ILifeTimeCommand 
    {
        private readonly UniTask<IContext> contextTask;
        private readonly AssetReference resource;
        
        public RegisterDataSourceCommand(UniTask<IContext> contextTask,AssetReference resource)
        {
            this.contextTask = contextTask;
            this.resource = resource;
        }

        public async void Execute(ILifeTime lifeTime)
        {
            var asset = await resource.LoadAssetTaskAsync<ScriptableObject>(lifeTime);
            var dataSource = asset as IAsyncContextDataSource;
            if (dataSource == null) {
                GameLog.LogError($"NULL asset loaded from {resource}");
                return;
            }
            
            await dataSource.RegisterAsync(await contextTask);

        }
    }
}
