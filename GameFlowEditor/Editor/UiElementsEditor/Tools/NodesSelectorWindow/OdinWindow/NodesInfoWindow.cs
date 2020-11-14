#if ODIN_INSPECTOR

namespace UniModules.UniGameFlow.GameFlowEditor.Editor.NodesSelectorWindow
{
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using UniGame.Core.EditorTools.Editor.AssetOperations;
    using UniGame.GameFlow.GameFlowEditor.Editor.NodesSelectorWindow.OdinWindow;
    using UnityEditor;

    
    public class NodesInfoWindow : OdinEditorWindow {
        
        private const string MainButtonsGroup = "maincommands";
        
        #region static initialization

        [MenuItem("UniGame/GameFlow/Nodes Search Window")]
        public static void OpenWindow() => ShowWindow();
        
        public static NodesInfoWindow ShowWindow() 
        {
            if (window)
            {
                window.Focus();
                return window;
            }
            
            window = GetWindow<NodesInfoWindow>();
            window.InitializeWindow();
            window.Show();
            
            return window;
        }

        [InitializeOnLoadMethod]
        public static void AutoReloadNodes() => window?.Reload();

        private static NodesInfoWindow window;

        public static NodesInfoWindow NodesWindow => window ? window : ShowWindow();

        #endregion

        #region inspector

#if ODIN_INSPECTOR
        [HideLabel]
        [InlineProperty]
#endif
        public NodesViewerEditor editor = new NodesViewerEditor();
        
        #endregion
        
        public void InitializeWindow()
        {
            this.editor = new NodesViewerEditor();
            this.Reload();
        }
        
        [HorizontalGroup(MainButtonsGroup,Order = 0)]
        [PropertyOrder(-1)]
        [Button]
        public void Reload()
        {
            this.editor.Reload();
        }

    }
}


#endif