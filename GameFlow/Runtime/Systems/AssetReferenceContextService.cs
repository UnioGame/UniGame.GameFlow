namespace UniModules.UniGameFlow.GameFlow.Runtime.Systems
{
    using System;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class AssetReferenceContextService : AssetReferenceT<ContextService>
    {
        public AssetReferenceContextService(string guid) : base(guid)
        {
        }
    }
}