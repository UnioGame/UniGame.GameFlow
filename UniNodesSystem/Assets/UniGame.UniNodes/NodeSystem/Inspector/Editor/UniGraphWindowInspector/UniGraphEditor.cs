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
				graph.Validate();
			}
			
		}

	}
}
