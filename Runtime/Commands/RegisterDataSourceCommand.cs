namespace UniGame.UniNodes.GameFlow.Runtime.Commands
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniContextData.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.ScriptableObjects;
    using UniRx;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class RegisterDataSourceCommand : ILifeTimeCommand
    {
        private readonly UniTask<IContext> _contextTask;
        private readonly AssetReference _resource;

        // TODO есть целый зоопарк наследников AssetReference этому конструктору на вход нужно получить
        // AssetReference по которому можно загрузить наследника LifeTimeScriptableObject приводимого к интерфейсу
        // IAsyncContextDataSource поскольку на вход принимаются реализации через синтаксис конструктора такое не реализовать,
        // возможно будет работать FactoryMethod
        public RegisterDataSourceCommand(UniTask<IContext> contextTask, AssetReference resource)
        {
            _contextTask = contextTask;
            _resource = resource;
        }

        public async void Execute(ILifeTime lifeTime)
        {
            if (_resource == null)
                return;
            var asset = await _resource.LoadAssetTaskAsync<LifetimeScriptableObject>(lifeTime);
            // HACK HOGM-6286
            await UniTask.WaitForEndOfFrame();
            if (!(asset is IAsyncContextDataSource dataSource))
            {
                GameLog.LogError($"Asset loaded by guid {_resource.AssetGUID} is not {nameof(IAsyncContextDataSource)} or NULL");
                _resource.UnloadReference();
                return;
            }

            OnSourceLoaded(asset, lifeTime);
            await dataSource.RegisterAsync(await _contextTask);
        }

        protected virtual void OnSourceLoaded(LifetimeScriptableObject asset, ILifeTime lifeTime)
        {

        }
    }
}
