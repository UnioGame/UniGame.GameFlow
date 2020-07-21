using UnityEngine;

namespace UniModules.UniGameFlow.GameFlow.Runtime.Services.Common
{
    using Systems;
    using Cysharp.Threading.Tasks;
    using UniGame.AddressableTools.Runtime.SpriteAtlases;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniGame.AddressableTools.Runtime.Extensions;
    

    [CreateAssetMenu(menuName = "UniGame/GameSystem/Services/AddressablesAtlasesService",fileName = nameof(AddressablesAtlasesService))]
    public class AddressablesAtlasesServiceAsset : ContextService<AddressablesAtlasesService>
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineEditor()]
#endif
        [SerializeField]
        private AddressableSpriteAtlasConfiguration _configuration;
        
        protected override async UniTask<AddressablesAtlasesService> CreateServiceInternalAsync(IContext context)
        {
            var service = new AddressablesAtlasesService(_configuration);
            return service;
        }
    }
}
