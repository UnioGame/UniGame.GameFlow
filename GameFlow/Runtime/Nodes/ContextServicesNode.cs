using UniGame.UniNodes.Nodes.Runtime.Common;

namespace UniModules.UniGameFlow.GameFlow.Runtime.Nodes
{
    using System.Collections.Generic;
    using Systems;
    using NodeSystem.Runtime.Core.Attributes;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniRx;
    using UniRx.Async;
    using UnityEngine;

    [CreateNodeMenu("GameSystem/ContextServicesNode")]
    public class ContextServicesNode : ContextNode
    {
        [SerializeField]
        private List<AssetReferenceContextService> _referenceServices = new List<AssetReferenceContextService>();

        [SerializeField]
        private List<ContextServiceAsset> _services = new List<ContextServiceAsset>();
        
        protected override async void OnContextActivate(IContext context)
        {
            await ExecuteServices();
        }
        
        private async UniTask<Unit> ExecuteServices()
        {
            foreach (var service in _services) {
                var disposable = await service.Execute(Source);
                LifeTime.AddDispose(disposable);
            }
            
            foreach (var serviceReference in _referenceServices) {
                var service    = await serviceReference.LoadAssetTaskAsync(LifeTime);
                var disposable = await service.Execute(Source);
                LifeTime.AddDispose(disposable);
            }
            
            return Unit.Default;
        }
    }
}
