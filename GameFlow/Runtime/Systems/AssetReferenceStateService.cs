namespace UniModules.UniGameFlow.GameFlow.Runtime.Systems
{
    using System;
    using UniGreenModules.UniGame.SerializableContext.Runtime.Addressables;

    [Serializable]
    public class AssetReferenceStateService : AssetReferenceScriptableObject<StateContextService>
    {
        public AssetReferenceStateService(string guid) : base(guid)
        {
        }
    }
}