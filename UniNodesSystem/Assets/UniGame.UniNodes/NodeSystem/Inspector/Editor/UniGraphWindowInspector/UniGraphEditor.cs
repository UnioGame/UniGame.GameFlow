namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector
{
	using BaseEditor;
	using Runtime.Core;
	using UnityEngine;

	[CustomNodeGraphEditor(typeof(UniGraph))]
	public class UniGraphEditor : NodeGraphEditor
	{
		private UniGraph graph;
		
		public override void OnEnable()
		{
			base.OnEnable();
			graph = Node as UniGraph;
			if (Application.isPlaying == false) {
				graph?.Validate();
			}
			
		}

	}
}
