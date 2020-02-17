using UnityEngine;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Examples.ReactivePortDemo
{
    using System.Collections;
    using System.Collections.Generic;
    using UniNodeSystem.Nodes;
    using UniRoutine.Runtime;
    using UniRoutine.Runtime.Extension;

    public class GraphTestGenerator : MonoBehaviour
    {
        public int count = 100;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineEditor()]
#endif
        public UniGraph target;

        public void Start()
        {
            for (int i = 0; i < count; i++) {
                //Awaiter(0.1f).Execute();
                Instantiate(target.gameObject);
            }
        }

        private IEnumerator<object> Awaiter(float delay)
        {
            while (isActiveAndEnabled) {
                yield return Awaiter();
                yield return Awaiter();
                yield return Awaiter();
                yield return Awaiter();
                yield return Awaiter();
                yield return Awaiter();
                yield return this.WaitForSeconds(delay);
            }
        }

        private IEnumerator Awaiter()
        {
            yield return null;
            yield return null;
            yield return null;
        }
    }
}