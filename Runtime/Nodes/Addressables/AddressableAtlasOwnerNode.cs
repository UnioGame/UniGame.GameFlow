using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniGame.UniNodes.Nodes.Runtime.Common;
using UniModules.UniGame.AddressableTools.Runtime.Extensions;
using UniModules.UniGame.AddressableTools.Runtime.SpriteAtlases;
using UniModules.UniGame.Core.Runtime.Interfaces;
using UniModules.UniGame.CoreModules.UniGame.Context.Runtime.Extension;
using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
using UnityEngine.AddressableAssets;

[CreateNodeMenu("Addressables/AddressableAtlasOwner",nodeName = "Addressable Atlas Owner")]
public class AddressableAtlasOwnerNode : ContextNode
{

    public List<AssetReferenceT<AddressableAtlasesStateAsset>> atlases = new List<AssetReferenceT<AddressableAtlasesStateAsset>>();

    protected override async UniTask OnContextActivate(IContext context)
    {
        var atlasHandler = await context.ReceiveFirstAsync<IAddressableSpriteAtlasHandler>(LifeTime);

        var tasks         = atlases.Select(x => x.LoadAssetTaskAsync(LifeTime));
        var atlasesAssets = await UniTask.WhenAll(tasks);

        foreach (var atlaseAsset in atlasesAssets)
        {
            atlasHandler.BindAtlasesLifeTime(LifeTime,atlaseAsset);
        }
        
        Complete();
    }
}
