namespace UniModules.UniGame.GameFlow.GameFlowEditor.Editor.UiElementsEditor.Tools.PortData
{
    using global::UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
    using UnityEditor;
    using UnityEngine;

    public class PortDataWindow :
#if ODIN_INSPECTOR
        Sirenix.OdinInspector.Editor.OdinEditorWindow
#else
        EditorWindow
#endif
        
    {
        #region static data

        private static PortDataWindow window;

        public static PortDataWindow Open(INode node)
        {
            window = window ?? GetWindow<PortDataWindow>();
            window.Initialize(node);
            window.Show();
            return window;
        }

        #endregion
        
        #region inspector
        
        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineProperty]
        [Sirenix.OdinInspector.HideLabel]
#endif
        private NodePortsViewerEditor nodesViewer = new NodePortsViewerEditor();
        
        #endregion

        private INode _node;
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
        [Sirenix.OdinInspector.PropertyOrder(-1)]
#endif
        public void Refresh()
        {
            nodesViewer.Initialize(_node);
        }
        
        public void Initialize(INode node)
        {
            _node = node;
            nodesViewer.Initialize(node);
        }
        
    }
}
