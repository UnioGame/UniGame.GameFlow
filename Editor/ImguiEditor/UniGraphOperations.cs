using UniGame.GameFlowEditor.Runtime;

namespace UniGame.GameFlow.Editor
{
    using System.IO;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.Editor;
    using UnityEditor;
    using UnityEngine;

    public class UniGraphOperations
    {
        private const string DefaultGraphName = "UniGraph";
        private const string DefaultInputNodeName = "input";

        [MenuItem("Assets/UniGraph/Create UniGraph")]
        public static void CreateGraph()
        {
            var graph = new GameObject().AddComponent<UniGraph>();
            graph.SetName(graph.name);

            //add main root node
            var root = graph.AddNode<UniPortNode>(DefaultInputNodeName);
            root.name = DefaultInputNodeName;
            root.direction = PortIO.Input;

            var activePath = AssetDatabase.GetAssetPath(Selection.activeObject);

            var assetFolder = Directory.Exists(activePath) ? activePath : Path.GetDirectoryName(activePath);
          
            var assetGameObject = graph.gameObject.SaveAsset(DefaultGraphName, assetFolder);
            graph = assetGameObject.GetComponent<UniGraph>();
            
            AssetDatabase.Refresh();

            var serializableGraph = ScriptableObject.CreateInstance<UniGraphAsset>();
            graph.serializedGraph = serializableGraph;
            serializableGraph.name = graph.name;
            serializableGraph.SaveAssetAsNested(graph.gameObject);
            graph.gameObject.MarkDirty();
        }
    }
}