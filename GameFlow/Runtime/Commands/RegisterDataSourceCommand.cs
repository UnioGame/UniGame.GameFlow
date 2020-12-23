namespace UniGame.UniNodes.GameFlow.Runtime.Commands
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniContextData.Runtime.Interfaces;
    using UniModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class RegisterDataSourceCommand : ILifeTimeCommand 
    {
        private readonly UniTask<IContext> _contextTask;
        private readonly AssetReference _resource;
        
        public RegisterDataSourceCommand(UniTask<IContext> contextTask, AssetReference resource)
        {
            _contextTask = contextTask;
            _resource = resource;
        }

        public async void Execute(ILifeTime lifeTime)
        {
            var asset = await _resource.LoadAssetTaskAsync<ScriptableObject>(lifeTime);
            if (!(asset is IAsyncContextDataSource dataSource)) {
                GameLog.LogError($"NULL asset loaded from {_resource}");
                return;
            }
            
            await dataSource.RegisterAsync(await _contextTask);
        }
    }
}
