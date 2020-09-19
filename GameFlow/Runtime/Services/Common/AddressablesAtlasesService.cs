namespace UniModules.UniGameFlow.GameFlow.Runtime.Services.Common
{
    using Cysharp.Threading.Tasks;
    using global::UniGame.UniNodes.GameFlow.Runtime;
    using UniGame.AddressableTools.Runtime.SpriteAtlases;
    using UniGame.AddressableTools.Runtime.SpriteAtlases.Abstract;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UnityEngine.SceneManagement;

    public class AddressablesAtlasesService : GameService, IAddressablesAtlasesService
    {
        private IAddressableSpriteAtlasHandler _addressableSpriteAtlasHandler;
        
        public AddressablesAtlasesService(IAddressableSpriteAtlasHandler spriteAtlasManager)
        {
            _addressableSpriteAtlasHandler = spriteAtlasManager;
            _addressableSpriteAtlasHandler.
                Execute().
                AddTo(LifeTime);

            Observable.FromEvent(
                x => SceneManager.activeSceneChanged += OnSceneChanged,
                x => SceneManager.activeSceneChanged -= OnSceneChanged).
                Subscribe().
                AddTo(LifeTime);

        }
        
        public async UniTask<bool> RequestSpriteAtlas(string guid)
        {
            return await _addressableSpriteAtlasHandler.RequestSpriteAtlas(guid);
        }

        private void OnSceneChanged(Scene fromScene, Scene toScene)
        {
            if (fromScene.path == toScene.path)
                return;
            return;
            _addressableSpriteAtlasHandler?.Unload();
        }
        
    }
}
