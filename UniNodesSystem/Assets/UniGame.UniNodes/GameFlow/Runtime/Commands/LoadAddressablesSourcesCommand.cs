namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.GameFlow.Runtime.Commands
{
    using System.Collections.Generic;
    using UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniRx.Async;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    public class LoadAddressablesSourcesCommand<TSource, TResult>
        where TResult : class
        where TSource : Object
    {
        private readonly IReadOnlyList<AssetReference> resources;

        private readonly List<TResult> sources = new List<TResult>();

        public IReadOnlyList<TResult> Sources => sources;

        public LoadAddressablesSourcesCommand(IReadOnlyList<AssetReference> resources)
        {
            this.resources = resources;
        }

        public async UniTask<IReadOnlyList<TResult>> Execute(ILifeTime lifeTime)
        {
            sources.Clear();
            return await resources.LoadAssetsTaskAsync<TSource, TResult, AssetReference>(sources);
        }
    }
}