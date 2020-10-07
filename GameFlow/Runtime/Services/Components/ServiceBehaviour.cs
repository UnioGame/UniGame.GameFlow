using UnityEngine;

namespace UniModules.UniGameFlow.GameFlow.Runtime.Systems.Components
{
    using Cysharp.Threading.Tasks;
    using UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniGame.Core.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniRx;
    

    public class ServiceBehaviour : MonoBehaviour, ILifeTimeContext
    {
        #region inspector
        
        [SerializeField]
        private bool _dontDestroy = false;
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineProperty]
        [Sirenix.OdinInspector.HideLabel]
#endif
        [SerializeField]
        public ContextServiceData _serviceData = new ContextServiceData();

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
            _serviceData.Execute(LifeTime);
            
            return Unit.Default;
        }
        
        private void OnDestroy()
        {
            _lifeTime.Terminate();
        }
    }
}
