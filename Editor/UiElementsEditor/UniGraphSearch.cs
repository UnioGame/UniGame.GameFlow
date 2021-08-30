namespace UniGame.UniNodes.GameFlowEditor.Editor
{
    using GraphProcessor;
    using UnityEngine.UIElements;

    public class UniGraphSearch : VisualElement
    {
        private readonly BaseGraphView _graphView;

        public UniGraphSearch(BaseGraphView graphView)
        {
            name       = nameof(UniGraphSearch);
            _graphView = graphView;
            
            Draw();
        }

        private void Draw()
        {
            
        }
    }
}