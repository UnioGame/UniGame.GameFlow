namespace UniGreenModules.UniNodeSystem.Inspector.Editor
{
	using System.IO;
	using System.Linq;
	using Boo.Lang;
	using Runtime;
	using Runtime.Core;
	using UniCore.EditorTools.Editor.AssetOperations;
	using UniCore.EditorTools.Editor.Utility;
	using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Nodes;
	using UniNodeSystem.Nodes;
	using UnityEditor;
	using UnityEngine;

	public class UniGraphOperations 
	{
		
		[MenuItem("Assets/UniGraph/Create UniGraph")]
		public static void CreateGraph()
		{
			
			var graph = new GameObject().AddComponent<UniGraph>();
			
			graph.Graph = graph;
			graph.nodeName = graph.name;
			
			//add main root node
			var root = graph.AddNode<UniPortNode>("input");
			root.name = "input";
			root.direction = PortIO.Input;
			
			var activePath = AssetDatabase.GetAssetPath(Selection.activeObject);
			
			var assetFolder = Directory.Exists(activePath) ? activePath :
				Path.GetDirectoryName(activePath);
			
			AssetEditorTools.SaveAsset(graph.gameObject, "UniGraph", assetFolder);
		}
	
	}
}
