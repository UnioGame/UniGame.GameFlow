using UnityEngine;

namespace UniModules.UniGameFlow.Nodes.Runtime.Addressables
{
    using System;
    using System.Collections.Generic;
    using global::UniGame.UniNodes.Nodes.Runtime.Common;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Attributes;
    using UniModules.UniCore.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.AddressableTools.Runtime.Extensions;
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

        private ReactiveProperty<TApi> _sourceValue = new ReactiveProperty<TApi>();
        private IDisposable _localSourceValueDisposable;
        
        public IReadOnlyReactiveProperty<TApi> SourceValue { get; }

        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);
            _sourceValue = new ReactiveProperty<TApi>();
            
            LifeTime.AddCleanUpAction(() => _localSourceValueDisposable.Cancel());
        }

        protected override async void OnContextActivate(IContext context)
        {
            _localSourceValueDisposable.Cancel();
            _localSourceValueDisposable = _sourceValue.
                Skip(1).
                Subscribe(x => OnValueChanged(x,context));
            
            var result = await _source.LoadAssetTaskAsync<T>(LifeTime);
            if (result is TApi value) {
                _sourceValue.Value = value;      
            }
        }

        protected virtual void OnValueChanged(TApi value, IContext context)
        {
            context.Publish(value);
            Complete();
        }
    }
}
