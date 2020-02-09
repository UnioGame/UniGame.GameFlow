namespace UniGreenModules.UniGameSystems.Examples.SimpleSystem
{
    using UniNodeSystem.Nodes;
    using UniNodeSystem.Runtime.Interfaces;
    using UnityEngine;

    public class GraphLauncher : MonoBehaviour
    {
        private IUniGraph targetGraph;

        public UniGraph graph;

        private void OnEnable()
        {
            targetGraph = graph ? GetComponent<IUniGraph>() : graph;
            targetGraph?.Execute();
        }

        private void OnDisable()
        {
            targetGraph?.Exit();
        }

    }
}
