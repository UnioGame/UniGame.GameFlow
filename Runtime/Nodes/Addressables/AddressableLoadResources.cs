using UniModules.UniGame.CoreModules.UniGame.AddressableTools.Runtime.Extensions;

namespace UniGame.UniNodes.Nodes.Runtime.Addressables
{
    using System.Collections.Generic;
    using Common;
    using Cysharp.Threading.Tasks;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    [CreateNodeMenu("Addressable/" + nameof(AddressableLoadResources),"AddressableLoadResources")]
    public class AddressableLoadResources : ContextNode
    {
        #region inspector
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        public List<AssetReference> assetReferences = new List<AssetReference>();

        #endregion

        protected override async UniTask OnContextActivate(IContext context)
        {
            await UniTask.WhenAll(assetReferences.Select(x => x.LoadAssetTaskAsync<Object>(LifeTime)));
            Complete();
        }
    }
}
