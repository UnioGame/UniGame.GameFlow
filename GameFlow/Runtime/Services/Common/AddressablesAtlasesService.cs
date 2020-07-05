namespace UniModules.UniGameFlow.GameFlow.Runtime.Services.Common
{
    using System;
    using global::UniGame.UniNodes.GameFlow.Runtime;
    using Interfaces;
    using UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.U2D;

    public class AddressablesAtlasesService : GameService
    {

        public AddressablesAtlasesService()
        {
            Observable.FromEvent(
                x => SpriteAtlasManager.atlasRequested += OnSpriteAtlasRequested,
                x => SpriteAtlasManager.atlasRequested -= OnSpriteAtlasRequested).Subscribe().
                AddTo(LifeTime);
        }


        private async void OnSpriteAtlasRequested(string tag, Action<SpriteAtlas> atlasAction)
        {
            GameLog.LogRuntime($"OnSpriteAtlasRequested : TAG {tag}", Color.blue);
            var result = await Addressables.LoadAssetAsync<SpriteAtlas>(tag).Task;
            if (result == null) {
                GameLog.LogRuntime("Null Atlas Result");
                return;
            }
            atlasAction(result);
        }
        
    }
}
