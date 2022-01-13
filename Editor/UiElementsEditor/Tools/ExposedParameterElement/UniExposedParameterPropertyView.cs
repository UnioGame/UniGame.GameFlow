using System;
using GraphProcessor;
using UniGame.GameFlowEditor.Runtime;
using UnityEngine.UIElements;

namespace UniModules.UniGame.GameFlow.Editor.UiElementsEditor.Tools.ExposedParameterElement
{
    [Serializable]
    public class UniExposedParameterPropertyView : VisualElement
    {
        protected BaseGraphView baseGraphView;

        public IUniExposedParameter parameter { get; private set; }

        public Toggle     hideInInspector { get; private set; }

        public UniExposedParameterPropertyView(BaseGraphView graphView, IUniExposedParameter param)
        {
            baseGraphView = graphView;
            parameter      = param;
        }
    }
}