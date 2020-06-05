namespace UniGame.UniNodes.NodeSystem.Runtime.Components
{
    using Core;
    using Interfaces;
    using UnityEngine;

    public class GraphLauncher : MonoBehaviour
    {
        private IUniGraph targetGraph;

        public UniGraph graph;

        private void Awake() => targetGraph = graph ? graph : GetComponent<IUniGraph>();

        private void Start() => targetGraph?.Execute();

        private void OnDisable() =>  targetGraph?.Exit();

    }
}
