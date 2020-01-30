namespace UniGreenModules.UniNodeSystem.Inspector.Editor
{
	using BaseEditor;
	using Runtime;
	using Runtime.Core;
	using UniNodeSystem.Nodes;
	using UnityEngine;

	[CustomNodeGraphEditor(typeof(UniGraph))]
	public class UniGraphEditor : NodeGraphEditor
	{

		private UniGraph graph;
		
		public override void OnEnable()
		{
			base.OnEnable();
			graph = target as UniGraph;
			CleanUp();
		}
		
		private void CleanUp()
		{
			var changed = false;
			var nodes = graph.nodes;
			foreach (Transform child in graph.transform) 
			{
				var node = child.gameObject.GetComponent<UniBaseNode>();
				if (nodes.Contains(node) != false) {
					continue;
				}
				RemoveNode(node);
				changed = true;
			}

			if (changed) {
				UnityEditor.EditorUtility.SetDirty(graph);
			}
		}
		
	}
}
