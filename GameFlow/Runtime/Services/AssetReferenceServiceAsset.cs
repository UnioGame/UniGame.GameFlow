namespace UniModules.UniGameFlow.GameFlow.Runtime.Systems
{
    using System;
    using Interfaces;
    using Services;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class AssetReferenceServiceAsset : AssetReferenceT<ServiceDataSourceAsset<IGameService>>
    {
        public AssetReferenceServiceAsset(string guid) : base(guid)
        {
        }
    }
}