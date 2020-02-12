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
			foreach (var node in graph.GetComponents<UniBaseNode>()) 
			{
				if (nodes.Contains(node) != false || graph == node) {
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
