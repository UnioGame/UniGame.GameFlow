using UniModules.UniGame.Core.Runtime.ScriptableObjects;

namespace UniModules.UniGameFlow.GameFlow.Runtime.Systems
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniGame.Core.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.Attributes;
    using UniRx;
    
    using UnityEngine;

    public abstract class BaseServiceAsset<TData> :
        LifetimeScriptableObject, 
        IAsyncState<TData,IDisposable>
        where TData : class
    {
        #region inspector

        [ReadOnlyValue]
        [SerializeField] 
        private bool _isActive;

        [HideInInspector]
        [SerializeField]
        private bool _isPlaying = false;
        
        #endregion

        private TData _observableSource;
        
        public bool IsActive => !_lifeTimeDefinition.IsTerminated;

        public async UniTask<IDisposable> ExecuteAsync(TData source)
        {
            if (Application.isPlaying == false) {
                return Disposable.Empty;
            }
            
            if(_isActive) return this;

            if (source == null)
                return this;
            
            _isActive = true;
            _observableSource = source;
            
            LifeTime.AddCleanUpAction(() => _observableSource = null);
            LifeTime.AddCleanUpAction(() => _isActive = false);

            await OnInitialize(source);
            
            return this;
        }

        public async UniTask ExitAsync() => Dispose();

        #region private methods

        protected abstract UniTask<Unit> OnInitialize(TData context);
        
        #if UNITY_EDITOR

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf("_isPlaying")]
        [Sirenix.OdinInspector.HideIf("_isActive")]
        [Sirenix.OdinInspector.Button]
#endif
        private async void Start()
        {
            await ExecuteAsync(_observableSource);
        }
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf("_isActive")]
        [Sirenix.OdinInspector.Button]
#endif
        private void Stop() => ExitAsync();
#endif

        protected override void OnActivate()
        {
            _isPlaying = Application.isPlaying;
        }

        #endregion
    }
}
