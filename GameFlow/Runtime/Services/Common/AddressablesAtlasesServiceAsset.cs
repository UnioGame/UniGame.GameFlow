using UnityEngine;

namespace UniModules.UniGameFlow.GameFlow.Runtime.Services.Common
{
    using Systems;
    using Cysharp.Threading.Tasks;
    using UniGame.AddressableTools.Runtime.SpriteAtlases;
    using UniGame.AddressableTools.Runtime.SpriteAtlases.Abstract;
    using UniGame.Core.Runtime.Interfaces;


    [CreateAssetMenu(menuName = "UniGame/GameSystem/Services/AddressablesAtlasesService",fileName = nameof(AddressablesAtlasesService))]
    public class AddressablesAtlasesServiceAsset : ServiceDataSourceAsset<IAddressablesAtlasesService>
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineEditor()]
#endif
        [SerializeField]
        private AddressableSpriteAtlasConfiguration _configuration;
        
        protected override async UniTask<IAddressablesAtlasesService> CreateServiceInternalAsync(IContext context)
        {
            var service = new AddressablesAtlasesService(_configuration);
            context.Publish<IAddressablesAtlasesLoader>(service);
            return service;
        }
    }
}
