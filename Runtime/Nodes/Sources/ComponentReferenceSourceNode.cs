using UniGame.AddressableTools.Runtime;
using UniGame.Context.Runtime;

namespace UniGame.GameFlow
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniNodes.Nodes.Runtime.Common;
    using UniModules.GameFlow.Runtime.Attributes;
    using Core.Runtime.Extension;
    using Core.Runtime;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [HideNode]
    [Serializable]
    public class ComponentReferenceSourceNode<TComponent> : SContextNode
        where TComponent : Component
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        public AssetReferenceComponent<TComponent> assetReference;

        public bool destroyWith = true;
        
        protected override async UniTask OnContextActivate(IContext context)
        {
            var component = await assetReference.LoadAssetTaskAsync(LifeTime);

            var instance = Object.Instantiate(component.gameObject);
            if(destroyWith) 
                instance.DestroyWith(LifeTime);

            var result = instance.GetComponent<TComponent>();
            context.Publish(result);
            
            Complete();
        }
    } 
}


