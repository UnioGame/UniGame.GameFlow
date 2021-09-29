namespace UniModules.GameFlow.Runtime.Components
{
    using Core;
    using Cysharp.Threading.Tasks;
    using Interfaces;
    using UniGame.Context.Runtime.Context;
    using UniGame.Core.Runtime.DataFlow.Extensions;
    using UnityEngine;

    public class GraphLauncher : MonoBehaviour
    {
        private IUniGraph targetGraph;

        public UniGraph graph;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void Stop()
        {
            targetGraph?.Exit();
        }
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void Execute()
        {
            var lifeTime = this.GetAssetLifeTime();
            targetGraph?.ExecuteAsync()
                .AttachExternalCancellation(lifeTime.TokenSource)
                .Forget();
        }
        
        private void Awake() => targetGraph = graph ? graph : GetComponent<IUniGraph>();

        private void Start() => Execute();

        private void OnDisable() =>  targetGraph?.Exit();

        
    }
}
