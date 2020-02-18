using UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Nodes;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Examples.SceneObjects
{
    using System.Collections;
    using UniRoutine.Runtime;
    using UniRoutine.Runtime.Extension;
    using UnityEngine;

    public class MoveTransformNode : UniNode
    {
        public Transform target;

        protected override void OnExecute()
        {
            Move().Execute().AddTo(lifeTime);
        }

        private IEnumerator Move()
        {
            var mult = 1;
            while (isActiveAndEnabled) {

                mult = target.position.y > 3 ? 1 : -1;
                target.position += new Vector3(0,mult * Time.deltaTime,0);
                yield return null;

            }
        }
        
    }
}
