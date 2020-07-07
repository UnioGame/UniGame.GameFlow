namespace UniModules.UniGameFlow.GameFlow.Runtime.Services.Common
{
    using global::UniGame.UniNodes.GameFlow.Runtime;
    using UniGame.AddressableTools.Runtime.SpriteAtlases;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UnityEngine.SceneManagement;

    public class AddressablesAtlasesService : GameService
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


        private void OnSceneChanged(Scene fromScene, Scene toScene)
        {
            if (fromScene.path == toScene.path)
                return;
            _addressableSpriteAtlasHandler?.Unload();
        }
        
    }
}
