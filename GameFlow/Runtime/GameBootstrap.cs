namespace UniModules.UniGame.GameFlow.GameFlow.Runtime 
{
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    public static class GameBootstrap {

        public const string BootLabel = "BootPoint";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static async void Initialize() {
            
            var bootPoint   = await Addressables.LoadAssetAsync<GameObject>(BootLabel).Task;
            if (!bootPoint)
            {
                Debug.LogWarning($"Boot GameManager Not Found with label {BootLabel}");
            }
            var gameManager = Object.Instantiate(bootPoint).GetComponent<Runtime.GameManager>();
            Object.DontDestroyOnLoad(gameManager.gameObject);
            
            await gameManager.Execute();
            
        }

    }
}
