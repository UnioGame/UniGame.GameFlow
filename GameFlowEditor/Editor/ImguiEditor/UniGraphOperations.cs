namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector
{
	using System.IO;
	using Runtime.Core;
	using UniGreenModules.UniCore.EditorTools.Editor.AssetOperations;
	using UnityEditor;
	using UnityEngine;

	public class UniGraphOperations 
	{
		
		[MenuItem("Assets/UniGraph/Create UniGraph")]
		public static void CreateGraph()
		{
			
			var graph = new GameObject().AddComponent<UniGraph>();
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
