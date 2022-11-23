using UniGame.AddressableTools.Runtime;
using UniGame.Context.Runtime;

namespace UniGame.UniNodes.GameFlow.Runtime.Commands
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniModules.UniCore.Runtime.DataFlow.Interfaces;
    using Core.Runtime;
    
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
            return await resources.LoadAssetsTaskAsync<TSource, TResult, AssetReference>(sources,lifeTime);
        }
    }
}