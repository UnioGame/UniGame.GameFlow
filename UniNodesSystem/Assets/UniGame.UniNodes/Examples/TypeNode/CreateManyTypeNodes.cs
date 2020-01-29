using UnityEngine;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Examples.TypeNode
{
    using UniCore.Runtime.ObjectPool.Runtime.Extensions;

    public class CreateManyTypeNodes : MonoBehaviour
    {
        public int count;

        public GameObject asset;
        
        // Start is called before the first frame update
        private void Start()
        {
            for (int i = 0; i < count; i++) {
                asset.Spawn();
            }
        }
    
    }
}
