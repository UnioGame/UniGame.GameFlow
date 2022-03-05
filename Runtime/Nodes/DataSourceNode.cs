namespace UniGame.UniNodes.GameFlow.Runtime.Nodes
{
    using Cysharp.Threading.Tasks;
    using UniModules.UniContextData.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.ScriptableObjects;
    using UniModules.UniGame.SerializableContext.Runtime.Addressables;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UniNodes.Nodes.Runtime.Common;
    using UnityEngine;

    [CreateNodeMenu("Common/Sources/Data Source")]
    public class DataSourceNode : ContextNode
    {
        public bool bindWithLifeTime = true;
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        public AssetReferenceDataSource contextDataSource;

        protected override async UniTask OnContextActivate(IContext context)
        {
            await base.OnContextActivate(context);

            var loadedRef = await contextDataSource.LoadAssetInstanceTaskAsync<LifetimeScriptableObject>(LifeTime);

            if (!(loadedRef is IAsyncContextDataSource asyncRef))
            {
                Debug.LogError($"Asset is not an IAsyncContextDataSource. Asset: '{loadedRef.Name}' | Graph: '{GraphData.ItemName}'");
                
                Complete();
                return;
            }
            
            await asyncRef.RegisterAsync(context);
            
            if (bindWithLifeTime)
            {
                loadedRef.AddTo(LifeTime);
            }
            
            Complete();
        }
    }
}