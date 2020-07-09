namespace Taktika.GameRuntime
{
    using System.Collections.Generic;
    using Abstract;
    using UniGame.UniNodes.NodeSystem.Runtime.Core;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGameFlow.GameFlow.Runtime.Services;
    using UniModules.UniGameFlow.GameFlow.Runtime.Systems;
    using UniRx;
    using UniRx.Async;
    using UnityEngine;

    public class GameManager : MonoBehaviour, IGameManager, ILifeTimeContext
    {
        #region inspector

        [SerializeField]
        private List<UniGraph> executionItems = new List<UniGraph>();

        [SerializeField]
        private List<AssetReferenceStateService> _referenceServices = new List<AssetReferenceStateService>();

        [SerializeField]
        private List<BaseContextService> _services = new List<BaseContextService>();
        
        #endregion
        
        private LifeTimeDefinition _lifeTimeDefinition = new LifeTimeDefinition();

        private IContext gameContext;
        
        #region public properties

        public static IGameManager Instance { get; protected set; }

        public IContext GameContext => gameContext;

        public ILifeTime LifeTime => _lifeTimeDefinition;
        
        #endregion
        
        #region public methods
        
        public void Initialize(IContext context)
        {
            gameContext = context;
            ExecuteGraphs();
            ExecuteServices();
        }

        #endregion
        
        #region private methods

        private void ExecuteGraphs()
        {
            foreach (var graph in executionItems) {
                graph.Execute(); 
            }
        }

        private async UniTask<Unit> ExecuteServices()
        {
            foreach (var service in _services) {
                var disposable = await service.Execute();
                LifeTime.AddDispose(disposable);
            }
            
            foreach (var serviceReference in _referenceServices) {
                var service = await serviceReference.LoadAssetTaskAsync(LifeTime);
                var disposable = await service.Execute();
                LifeTime.AddDispose(disposable);
            }
            
            return Unit.Default;
        }

        private void Awake()
        {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }

        private void OnDestroy() => _lifeTimeDefinition.Terminate();

        #endregion

    }
}
