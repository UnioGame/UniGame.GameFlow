namespace UniModules.UniGameFlow.GameFlowEditor.Editor.Tools
{
    using System;
    using global::UniGame.Core.EditorTools.Editor.UiElements;
    using global::UniGame.Core.Runtime.Attributes.FieldTypeDrawer;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Core;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Core.Nodes;
    using UnityEngine;
    using UnityEngine.UIElements;

    [UiElementsDrawer(1000)]
    public class SerializableNodeContaienrDrawer : IUiElementsTypeDrawer
    {
        private static Type containerType = typeof(SerializableNodeContainer);
        private Color _backgroundColor = new Color(0.4f, 0.4f, 0.4f);

        public bool IsTypeSupported(Type type)
        {
            return containerType.IsAssignableFrom(type);
        }

        public VisualElement Draw(object source, Type type, string label = "", Action<object> onValueChanged = null)
        {
            var container = source as SerializableNodeContainer;
            if(container == null)
                return new VisualElement();

            var node = container.node;
            var view = node.DrawNodeUiElements();
            view.style.backgroundColor = new StyleColor(_backgroundColor);
            view.style.paddingTop      = 4;
            view.style.paddingLeft     = 4;
            view.style.marginBottom    = 4;

            return view;
        }
        
    }
}
