﻿namespace UniGame.UniNodes.GameFlow.Runtime.Nodes
{
    using Cysharp.Threading.Tasks;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using AddressableTools.Runtime;
    using Core.Runtime;
    using Core.Runtime.ScriptableObjects;
    using Context.Runtime;
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

            var loadedRef = await contextDataSource
                .LoadAssetInstanceTaskAsync<LifetimeScriptableObject>(LifeTime,true);

            if (!(loadedRef is IAsyncDataSource asyncRef))
            {
                Debug.LogError($"Asset is not an IAsyncContextDataSource. Asset: '{loadedRef.Name}' | Graph: '{GraphData.ItemName}'");
                
                CompleteProcessing(context);
                return;
            }
            
            await asyncRef.RegisterAsync(context);
            
            if (bindWithLifeTime)
            {
                loadedRef.AddTo(LifeTime);
            }
            
            CompleteProcessing(context);
        }
    }
}