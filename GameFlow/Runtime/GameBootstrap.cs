namespace UniModules.UniGame.GameFlow.GameFlow.Runtime 
{
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    public static class GameBootstrap {

        public const string BootLabale = "BootPoint";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static async void Initialize() {

            var bootPoint   = await Addressables.LoadAssetAsync<GameObject>(BootLabale).Task;
            var gameManager = Object.Instantiate(bootPoint).GetComponent<Runtime.GameManager>();
            Object.DontDestroyOnLoad(gameManager.gameObject);
            
            await gameManager.Execute();
            
        }

    }
}
