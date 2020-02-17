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
			if (Application.isPlaying == false) {
				Validate();
			}
			
		}
		
		private void Validate()
		{
			var changed = false;
			var nodes = graph.nodes;
			foreach (var node in graph.GetComponents<Node>()) 
			{
				if (nodes.Contains(node) || graph == node) {
					continue;
				}
				RemoveNode(node);
				changed = true;
			}

			foreach (var node in nodes) {
				if (node.graph != null) {
					continue;
				}

				changed    = true;
				node.graph = graph;
			}

			if (changed) {
				UnityEditor.EditorUtility.SetDirty(graph);
			}
		}
		
	}
}
