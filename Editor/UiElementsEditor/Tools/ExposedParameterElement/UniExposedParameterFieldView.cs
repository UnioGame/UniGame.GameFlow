using System;
using UniGame.GameFlowEditor.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniModules.UniGame.GameFlow.Editor.UiElementsEditor.Tools.ExposedParameterElement
{
    [Serializable]
    public class UniExposedParameterFieldView : BlackboardField
    {
        protected UniGraphAsset	graphView;
        private readonly Action<IUniExposedParameter> _removeAction;

        public IUniExposedParameter	parameter { get; private set; }

        public UniExposedParameterFieldView(UniGraphAsset graphView, IUniExposedParameter param, Action<IUniExposedParameter> removeAction) : base(null, param.DisplayName, string.Empty)
        {
            this.graphView = graphView;
            _removeAction = removeAction;
            parameter = param;
            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
            this.Q("icon").AddToClassList("parameter-" + param.Info);
            this.Q("icon").visible = true;

            (this.Q("textField") as TextField).RegisterValueChangedCallback((e) => {
                param.DisplayName = e.newValue;
                text = e.newValue;
            });

            var addButton = new Button(ApplyParameter);
            addButton.text = "+";
            Add(addButton);
        }

        public void ApplyParameter()
        {
            parameter.Apply(graphView);
            //graphView.sourceGraph.ReloadEditor();
        }

        void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Rename", (a) => OpenTextEditor(), DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Delete", (a) => _removeAction(parameter), DropdownMenuAction.AlwaysEnabled);
            evt.StopPropagation();
        }
    }
}