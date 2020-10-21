using UnityEngine;

namespace UniModules.UniGameFlow.GameFlow.Runtime.Services.Common
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniGame.AddressableTools.Runtime.AssetReferencies;
    using UniGame.AddressableTools.Runtime.SpriteAtlases.Abstract;
    using UniGame.Context.Runtime.Abstract;
    using UniGame.Core.Runtime.Interfaces;
    using UniRx;

    [CreateAssetMenu(menuName = "UniGame/GameSystem/Services/AddressablesAtlasesContextLoader",fileName = nameof(AddressablesAtlasesContextLoader))]
    public class AddressablesAtlasesContextLoader : AsyncContextDataSource
    {
        public List<AssetReferenceSpriteAtlas> _loadedAtlases = new List<AssetReferenceSpriteAtlas>();
        
        public override async UniTask<IContext> RegisterAsync(IContext context)
        {
            var atlasLoader = await context.Receive<IAddressablesAtlasesLoader>().First();
            foreach (var assetReferenceSpriteAtlas in _loadedAtlases) {
                if (assetReferenceSpriteAtlas.RuntimeKeyIsValid() == false) {
                    Debug.LogError($"MISSING ATLAS AT {name}");
                    continue;
                }

                await atlasLoader.RequestSpriteAtlas(assetReferenceSpriteAtlas.AssetGUID);
            }

            return context;
        }
        
    }
}
