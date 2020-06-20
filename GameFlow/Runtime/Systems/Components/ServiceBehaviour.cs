using UnityEngine;

namespace UniModules.UniGameFlow.GameFlow.Runtime.Systems.Components
{
    using System;
    using System.Collections.Generic;
    using UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniGame.Core.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniGreenModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniRx;
    using UniRx.Async;

    public class ServiceBehaviour : MonoBehaviour, ILifeTimeContext
    {
        #region inspector
        
        [SerializeField]
        private bool _dontDestroy = false;
        
        [SerializeField]
        private List<AssetReferenceService> _referenceServices = new List<AssetReferenceService>();

        [SerializeField]
        private List<StateContextService> _services = new List<StateContextService>();

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
            foreach (var service in _services) {
                var disposable = await service.Execute();
                _lifeTime.AddDispose(disposable);
            }
            
            foreach (var serviceReference in _referenceServices) {
                var service    = await serviceReference.LoadAssetTaskAsync(_lifeTime);
                var disposable = await service.Execute();
                _lifeTime.AddDispose(disposable);
            }
            
            return Unit.Default;
        }
        
        private void OnDestroy()
        {
            _lifeTime.Terminate();
        }
    }
}
