namespace UniGreenModules.UniGameSystems.Examples.SimpleSystem
{
    using System;
    using UniNodeSystem.Nodes;
    using UniNodeSystem.Runtime.Interfaces;
    using UnityEngine;

    public class GraphLauncher : MonoBehaviour
    {
        private IUniGraph targetGraph;

        public UniGraph graph;

        private void Awake()
        {
            targetGraph = graph ? graph : GetComponent<IUniGraph>();
        }

        private void Start()
        {
            targetGraph?.Execute();
        }

        private void OnDisable()
        {
            targetGraph?.Exit();
        }

    }
}
