using System;
using UniModules.UniGame.AddressableTools.Runtime.SpriteAtlases;
using UnityEngine.AddressableAssets;


[Serializable]
#if ODIN_INSPECTOR
[Sirenix.OdinInspector.DrawWithUnity]
#endif
public class AssetReferenceAtlasState : AssetReferenceT<AddressableAtlasesStateAsset>
{
    public AssetReferenceAtlasState(string guid) : base(guid)
    {
    }
}