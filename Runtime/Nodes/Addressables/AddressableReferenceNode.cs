using UniGame.AddressableTools.Runtime;
using UniGame.Context.Runtime;
using UnityEngine;

namespace UniModules.UniGameFlow.Nodes.Runtime.Addressables
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using global::UniGame.UniNodes.Nodes.Runtime.Common;
    using global::UniModules.GameFlow.Runtime.Attributes;
    using global::UniGame.Core.Runtime;
    using UniGame.Core.Runtime.Rx;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UnityEngine.AddressableAssets;
    using Object = Object;

    [HideNode]
    [Serializable]
    public class AddressableReferenceNode<T> : AddressableReferenceNode<T,T> 
        where T : Object
    {
    }
    
    [HideNode]
    [Serializable]
    public class AddressableReferenceNode<T,TApi> : ContextNode
        where T : Object,TApi
        where TApi : class
    {
        [SerializeField]
        private AssetReference _source;

        protected override async UniTask OnContextActivate(IContext context)
        {
            var result = await _source.LoadAssetTaskAsync<T>(LifeTime);
            OnValueChanged(result, context);
        }

        protected virtual void OnValueChanged(TApi value, IContext context)
        {
            if (value != null)
            {
                context.Publish(value);
            }
            Complete();
        }
    }
}
