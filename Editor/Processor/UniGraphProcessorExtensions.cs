namespace UniModules.UniGameFlow.GameFlowEditor.Editor.Processor
{
    using Extensions;
    using global::UniModules.GameFlow.Runtime.Interfaces;
    using NodeSystem.Runtime.Extensions;
    using UniGame.Core.EditorTools.Editor;
    using UniModules.Editor;
    using UniRx;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    public static class UniGraphProcessorExtensions
    {
        private static NodeProcessor nodeProcessor;
        private static string        defaultProcessorPath = 
            EditorPathConstants.GeneratedContentPath.CombinePath(UniGraphEditorConstants.ProcessorPath);
        
        static UniGraphProcessorExtensions()
        {
            UniGraphEvent.NodeUpdateStream.Subscribe(UpdatePorts);
        }
        
        public static NodeProcessor NodeProcessor => GetNodeProcessor();

        public static NodeProcessor GetNodeProcessor() => (nodeProcessor = nodeProcessor == null ? nodeProcessor.LoadOrCreate(defaultProcessorPath) : nodeProcessor);

        public static void UpdatePorts(this INode node)
        {
            if (Application.isPlaying)
                return;
            
            NodeProcessor.UpdatePorts(node);
        }

    }
}
