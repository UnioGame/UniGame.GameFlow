namespace UniGame.UniNodes.GameFlow.Runtime.Commands
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.ScriptableObjects;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class OwnerRegisterDataSourceCommand : RegisterDataSourceCommand
    {
        public OwnerRegisterDataSourceCommand(UniTask<IContext> contextTask, AssetReference resource) : base(contextTask, resource)
        {
        }

        protected override void OnSourceLoaded(LifetimeScriptableObject asset, ILifeTime lifeTime)
        {
            base.OnSourceLoaded(asset, lifeTime);
            asset.AddTo(lifeTime);
        }
    }
}