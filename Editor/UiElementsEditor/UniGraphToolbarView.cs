using GraphProcessor;

namespace UniGame.UniNodes.GameFlowEditor.Editor
{
    using System;
    using System.Linq;
    using UnityEditor.Graphs;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class UniGraphToolbarView : ToolbarView
    {
        private const  string ActionMenu   = "Show Actions";
        private const  string elementName  = "title";

        public UniGraphToolbarView(BaseGraphView graphView) :
            base(graphView)
        {
        }

        public void AddTogglePinnedViewButton<TView>(string label,bool defaultStatus)
            where TView : PinnedElementView
        {
            AddToggleButton(label, defaultStatus, (v) => graphView.ToggleView<TView>());
        }
        
        public void AddToggleButton(string label,bool defaultStatus,Action<bool> action)
        {
            AddToggle(label, defaultStatus, action);
        }

        protected override void AddButtons()
        {
            var isVisible = graphView.GetPinnedElementStatus<UniGraphSettingsPinnedView>() !=
                            DropdownMenuAction.Status.Hidden;

            AddToggle(ActionMenu, isVisible, (v) => graphView.ToggleView<UniGraphSettingsPinnedView>());

            AddSearch();
        }

        private void AddSearch()
        {
            var inputField = new TextField();
            inputField.RegisterValueChangedCallback(x => FilterGraphObjects(x.newValue));

            Add(inputField);
        }

        private void FilterGraphObjects(string filter)
        {
            var root = graphView;
            root.Query<BaseNodeView>().Build().ForEach(x => FilterNodes(x, filter));
        }

        private static void FilterNodes(BaseNodeView nodeView, string filter)
        {
            var element    = nodeView.Q(elementName);
            var color      = element.style.backgroundColor;
            var firstChild = element.Children().FirstOrDefault();
            if (firstChild == null) return;

            var isValid = nodeView.nodeTarget.name.IndexOf(filter,StringComparison.InvariantCultureIgnoreCase) >= 0;
            isValid                          |= nodeView.nodeTarget.GetType().Name.IndexOf(filter,StringComparison.InvariantCultureIgnoreCase) >= 0;
            firstChild.style.backgroundColor =  !isValid || string.IsNullOrEmpty(filter) ? color : new StyleColor(Color.cyan) ;
        }
    }
}