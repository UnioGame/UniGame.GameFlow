namespace Taktika.GameResources.Addressables
{
    using System;
    using UniGreenModules.UniGame.SerializableContext.Runtime.Addressables;
    using UniModules.UniGameFlow.GameFlow.Runtime.Interfaces;
    using UniModules.UniGameFlow.GameFlow.Runtime.Systems;
    using UnityEngine;

    [Serializable]
    public class AssetReferenceService<TService>: 
        AssetReferenceScriptableObject<ScriptableObject, ContextServiceAsset<TService>>
        where TService : class, IGameService
    {
        public AssetReferenceService(string guid) : base(guid)
        {
        }
    }
}