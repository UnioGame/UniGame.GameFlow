using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniModules.UniGame.AddressableTools.Runtime.Extensions;
using UniModules.UniGame.AddressableTools.Runtime.SpriteAtlases;
using UniModules.UniGame.Context.SerializableContext.Runtime.States;
using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
using UniModules.UniGame.Core.Runtime.Interfaces;
using UniModules.UniGame.CoreModules.UniGame.Context.Runtime.Extension;
using UnityEngine.AddressableAssets;


#if ODIN_INSPECTOR
[Sirenix.OdinInspector.InlineProperty]
[Sirenix.OdinInspector.HideLabel]
#endif
[Serializable]
public class AddressableAtlasHandle : AsyncSharedContextState<IContext>
{
    public List<AssetReferenceT<AddressableAtlasesStateAsset>> atlases = new List<AssetReferenceT<AddressableAtlasesStateAsset>>();
    
    protected override async UniTask<IContext> OnExecute(IContext context, ILifeTime executionLifeTime)
    {
        var atlasHandler = await context.ReceiveFirstAsync<IAddressableSpriteAtlasHandler>(executionLifeTime);

        var tasks         = atlases.Select(x => x.LoadAssetTaskAsync(executionLifeTime));
        var atlasesAssets = await UniTask.WhenAll(tasks);

        foreach (var atlaseAsset in atlasesAssets)
        {
            atlasHandler.BindAtlasesLifeTime(LifeTime,atlaseAsset);
        }

        return context;
    }
}