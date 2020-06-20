namespace UniModules.UniGameFlow.GameFlow.Runtime.Systems
{
    using System;
    using UniGreenModules.UniGame.SerializableContext.Runtime.Addressables;

    [Serializable]
    public class AssetReferenceService : AssetReferenceScriptableObject<StateContextService>
    {
        public AssetReferenceService(string guid) : base(guid)
        {
        }
    }
}