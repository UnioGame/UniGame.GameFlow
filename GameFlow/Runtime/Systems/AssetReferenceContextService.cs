namespace UniModules.UniGameFlow.GameFlow.Runtime.Systems
{
    using System;
    using Interfaces;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class AssetReferenceContextService : AssetReferenceT<ContextService<IGameService>>
    {
        public AssetReferenceContextService(string guid) : base(guid)
        {
        }
    }
}