using UniGame.AddressableTools.Runtime;
using UniGame.Context.Runtime;

namespace UniGame.UniNodes.GameFlow.Runtime.Commands
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.ProfilerTools;
    using Context.Runtime;
    using UniModules.UniCore.Runtime.DataFlow;
    using Core.Runtime;
    using Core.Runtime.ScriptableObjects;
    using UniRx;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class RegisterDataSourceCommand : ILifeTimeCommand
    {
        private UniTask<IContext> _contextTask;
        private AssetReference    _resource;

        // TODO есть целый зоопарк наследников AssetReference этому конструктору на вход нужно получить
        // AssetReference по которому можно загрузить наследника LifeTimeScriptableObject приводимого к интерфейсу
        // IAsyncContextDataSource поскольку на вход принимаются реализации через синтаксис конструктора такое не реализовать,
        // возможно будет работать FactoryMethod
        public RegisterDataSourceCommand(UniTask<IContext> contextTask, AssetReference resource)
        {
            _contextTask = contextTask;
            _resource = resource;
        }

        public async UniTask Execute(ILifeTime lifeTime)
        {
            if (_resource == null || _resource.RuntimeKeyIsValid() == false)
            {
#if UNITY_EDITOR
                GameLog.LogWarning($"{nameof(RegisterDataSourceCommand)} : {nameof(_resource)} is empty");
#endif
                return;
            }
            
            var asset = await _resource.LoadAssetTaskAsync<LifetimeScriptableObject>(lifeTime);

            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            
            if (!(asset is IAsyncDataSource dataSource))
            {
                GameLog.LogError($"Asset loaded by guid {_resource.AssetGUID} is not {nameof(IAsyncDataSource)} or NULL");
                _resource.UnloadReference();
                return;
            }

            OnSourceLoaded(asset, lifeTime);
            
            dataSource.RegisterAsync(await _contextTask)
                .AttachExternalCancellation(lifeTime.CancellationToken)
                .Forget();
        }

        protected virtual void OnSourceLoaded(LifetimeScriptableObject asset, ILifeTime lifeTime)
        {

        }
    }
}
