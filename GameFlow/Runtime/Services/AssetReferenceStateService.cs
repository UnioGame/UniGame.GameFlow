namespace UniModules.UniGameFlow.GameFlow.Runtime.Systems
{
    using System;
    using Services;
    using UniModules.UniGame.SerializableContext.Runtime.Addressables;

    [Serializable]
    public class AssetReferenceStateService : AssetReferenceScriptableObject<ServiceDataSourceAsset>
    {
        public AssetReferenceStateService(string guid) : base(guid)
        {
        }
    }
}