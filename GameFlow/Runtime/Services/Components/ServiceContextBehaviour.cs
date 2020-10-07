using UnityEngine;

namespace UniModules.UniGameFlow.GameFlow.Runtime.Systems.Components
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniGame.Core.Runtime.Interfaces;
    using UniGame.SerializableContext.Runtime.Addressables;
    using UniGame.SerializableContext.Runtime.AssetTypes;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniCore.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniModules.UniGame.SerializableContext.Runtime.Addressables;
    using UniRx;
    

    public class ServiceContextBehaviour : MonoBehaviour, ILifeTimeContext
    {
        #region inspector
        
        [SerializeField]
        private bool _dontDestroy = false;
        [SerializeField]
        private AssetReferenceContextContainer contextReference;
        [SerializeField]
        private ContextContainerAsset contextContainer;
        [SerializeField]
        private ContextAsset contextAsset;
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineProperty]
        [Sirenix.OdinInspector.HideLabel]
#endif
        [SerializeField]
        public ContextServiceData serviceData = new ContextServiceData();

        #endregion

        private LifeTimeDefinition _lifeTime = new LifeTimeDefinition();

        public ILifeTime LifeTime => _lifeTime;

        private async void Start()
        {
            if (_dontDestroy) {
                DontDestroyOnLoad(gameObject);
            }

            await ExecuteServices();
        }
        
        private async UniTask<Unit> ExecuteServices()
        {
            var context = await LoadContext();

            serviceData.ExecuteServices(context, LifeTime);
            
            return Unit.Default;
        }
        
        
        private async UniTask<IObservable<IContext>> LoadContext()
        {
            if (contextAsset) return Observable.Return(contextAsset.Value);
            if (contextContainer) {
                return contextContainer;
            }

            return await contextReference.LoadAssetTaskAsync(LifeTime);
        }
        
        private void OnDestroy()
        {
            _lifeTime.Terminate();
        }
    }
}
