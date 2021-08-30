
namespace Game.Modules.Assets.UniGame.GameFlow.GameFlow.Runtime.Nodes.Common
{
    using Cysharp.Threading.Tasks;
    using global::UniGame.UniNodes.Nodes.Runtime.Common;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [CreateNodeMenu("Common/Scenes/LoadScene",nodeName = "LoadScene")]
    public class LoadSceneNode : ContextNode
    {
        [SerializeField]
        private int _sceneIndex;

        [SerializeField]
        private LoadSceneMode _loadMode = LoadSceneMode.Single;

        [SerializeField]
        private bool _loadAsync = false;
    
        protected override async UniTask OnContextActivate(IContext context)
        {
            if (_loadAsync)
            {
                await SceneManager.LoadSceneAsync(_sceneIndex, _loadMode);
                return;
            }
        
            SceneManager.LoadScene(_sceneIndex, _loadMode);
        }

    }
}
