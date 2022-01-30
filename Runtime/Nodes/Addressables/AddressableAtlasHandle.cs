using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniModules.UniGame.Context.SerializableContext.Runtime.States;
using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
using UniModules.UniGame.Core.Runtime.Interfaces;
using UniModules.UniGame.AddressableTools.Runtime.SpriteAtlases;
using UniModules.UniGame.CoreModules.UniGame.Context.Runtime.Extension;
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