using UniGame.UniNodes.NodeSystem.Runtime.Core;

namespace UniGame.UniNodes.Nodes.Runtime.Addressables
{
    using System.Collections.Generic;
    using NodeSystem.Runtime.Extensions;
    using NodeSystem.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniGame.SerializableContext.Runtime.Abstract;
    using UniRx;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
 
    [CreateNodeMenu("Addressables/UnloadAssetReferences", nodeName = "UnloadAssetReferences")]
    public class UnloadAssetReferencesNode : UniNode
    {
        private const string portName = "in";

        #region inspector
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        public List<AssetReference> assetReferences = new List<AssetReference>();

        [Header("Call Unload target resource on Desstroy Event")]
        public bool releaseOnDestroy = true;
        
        #endregion

        private IPortValue port;
        
        public void UnloadResources()
        {
            foreach (var reference in assetReferences) {
                if(reference.Asset == null)
                    continue;
                
                var targetAsset = reference.Asset;
                
                LogMessage($"UNLOAD AssetReference {targetAsset.name} : {reference.AssetGUID}");
                
                if (targetAsset is IResourceDisposable resourceDisposable) {
                    resourceDisposable.Dispose();
                }
                
                reference.ReleaseAsset();
            }
        }
        
        protected override void OnInitialize()
        {
            port = this.UpdatePortValue(portName, PortIO.Input);
        }

        protected override void OnExecute()
        {
            port.PortValueChanged.
                Where(x => port.HasValue).
                Do(x => UnloadResources()).
                Subscribe().
                AddTo(LifeTime);
        }

        protected void OnDestroy()
        {
            if(releaseOnDestroy)
                UnloadResources();
        }
    }
}
