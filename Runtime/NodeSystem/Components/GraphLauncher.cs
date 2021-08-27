namespace UniModules.GameFlow.Runtime.Components
{
    using Core;
    using Interfaces;
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
            targetGraph?.Execute();
        }
        
        private void Awake() => targetGraph = graph ? graph : GetComponent<IUniGraph>();

        private void Start() => targetGraph?.Execute();

        private void OnDisable() =>  targetGraph?.Exit();

        
    }
}
