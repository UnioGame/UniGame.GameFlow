namespace UniModules.UniGameFlow.GameFlowEditor.Editor.Processor
{
    using Extensions;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
    using NodeSystem.Runtime.Extensions;
    using UniGame.Core.EditorTools.Editor;
    using UniGame.Core.EditorTools.Editor.AssetOperations;
    using UniGame.Core.EditorTools.Editor.Tools;
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

        public static NodeProcessor GetNodeProcessor() => (nodeProcessor = nodeProcessor.LoadOrCreate(defaultProcessorPath));

        public static void UpdatePorts(this INode node)
        {
            if (Application.isPlaying)
                return;
            
            NodeProcessor.UpdatePorts(node);
        }

    }
}
