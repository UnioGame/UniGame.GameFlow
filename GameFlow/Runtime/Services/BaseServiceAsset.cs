using UniModules.UniGame.Core.Runtime.ScriptableObjects;

namespace UniModules.UniGameFlow.GameFlow.Runtime.Systems
{
    using System;
    using Sirenix.OdinInspector;
    using UniGreenModules.UniStateMachine.Runtime.Interfaces;
    using UniRx;
    using UniRx.Async;
    using UnityEngine;

    public abstract class BaseServiceAsset<TData> : 
        DisposableScriptableObject, 
        IAsyncState<IDisposable,TData>
        where TData : class
    {
        #region inspector

        [ReadOnly]
        [SerializeField] 
        private bool _isActive;

        #endregion

        private TData _observableSource;
        
        public bool IsActive => !_lifeTimeDefinition.IsTerminated;

        public async UniTask<IDisposable> Execute(TData source)
        {
            if (Application.isPlaying == false) {
                return Disposable.Empty;
            }
            
            if(_isActive) return this;

            Reset();
            
            if (source == null)
                return this;
            
            _isActive = true;
            _observableSource = source;
            
            LifeTime.AddCleanUpAction(() => _observableSource = null);
            LifeTime.AddCleanUpAction(() => _isActive = false);

            await OnInitialize(source);
            
            return this;
        }

        public void Exit() => Dispose();

        #region private methods

        protected abstract UniTask<Unit> OnInitialize(TData context);

        protected sealed override void OnDispose() => _lifeTimeDefinition.Terminate();

        #if UNITY_EDITOR

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        private async void Start()
        {
            await Execute(_observableSource);
        }
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        private void Stop() => Exit();
#endif

        #endregion
    }
}
