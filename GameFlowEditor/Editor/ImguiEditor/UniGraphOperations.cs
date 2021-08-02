namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector
{
	using System.IO;
	using Runtime.Core;
	using UniModules.Editor;
	using UnityEditor;
	using UnityEngine;

	public class UniGraphOperations
	{
		private const string DefaultGraphName     = "UniGraph";
		private const string DefaultInputNodeName = "input";
		
		[MenuItem("Assets/UniGraph/Create UniGraph")]
		public static void CreateGraph()
		{
			
			var graph = new GameObject().AddComponent<UniGraph>();
			graph.SetName(graph.name);
			
			//add main root node
			var root = graph.AddNode<UniPortNode>(DefaultInputNodeName);
			root.name      = DefaultInputNodeName;
			root.direction = PortIO.Input;
			
			var activePath = AssetDatabase.GetAssetPath(Selection.activeObject);
			
			var assetFolder = Directory.Exists(activePath) ? activePath :
				Path.GetDirectoryName(activePath);
			
			AssetEditorTools.SaveAsset(graph.gameObject, DefaultGraphName, assetFolder);
		}
	
	}
}
