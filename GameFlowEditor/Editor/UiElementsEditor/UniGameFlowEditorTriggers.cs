namespace UniModules.UniGame.GameFlow.GameFlowEditor.Editor.UiElementsEditor {
    using System.Collections.Generic;
    using System.Linq;
    using Core.EditorTools.Editor.AssetOperations;
    using Core.Runtime.Extension;
    using global::UniCore.Runtime.ProfilerTools;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Core;
    using UniRx;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public static class UniGameFlowEditorTriggers {

        [InitializeOnLoadMethod]
        public static void Initialize() {
            
            Selection.selectionChanged += SelectionChangedAction;
            
            //save graph when ctrl + s pressed
            Observable.FromEvent(
                    ev => EditorSceneManager.sceneSaved += OnSceneSaved,
                    ev => EditorSceneManager.sceneSaved -= OnSceneSaved).
                Subscribe();

            //redraw editor if assembly reloaded
            Observable.FromEvent(
                    ev => AssemblyReloadEvents.afterAssemblyReload += OnAssemblyReloaded,
                    ev => AssemblyReloadEvents.afterAssemblyReload -= OnAssemblyReloaded).
                Subscribe();

            Observable.FromEvent(
                    ev => AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload,
                    ev => AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload).
                Subscribe();

            Observable.FromEvent(
                    ev => EditorApplication.playModeStateChanged += OnPlayModeChanged,
                    ev => EditorApplication.playModeStateChanged -= OnPlayModeChanged).
                Subscribe();
        }

        public static void Reload() 
        {
            GetActiveWindows.ForEach(x => x.Reload());
        }

        public static void Save() {
            GetActiveWindows.ForEach(x => x.Save());
        }

        public static IEnumerable<UniGameFlowWindow> GetActiveWindows = UniGameFlowWindow.Windows.Where(x => x.IsEmpty == false);
        
        public static void SelectionChangedAction() {
            foreach (var graph in SelectGraphAssets()) {
                Select(graph);
            }
        }

        public static void Select(UniGraph uniGraph) {


            var window = UniGameFlowWindow.Windows.
                FirstOrDefault(x => x.IsEmpty);
            if (window == null) {
                return;
            }
            
            window.Initialize(uniGraph);

        }

        private static IEnumerable<UniGraph> SelectGraphAssets() 
        {
            var selectedGraphs = Selection.objects.
                Concat(Selection.gameObjects).
                Distinct().
                OfType<GameObject>().
                Select(x => x.GetComponent<UniGraph>()).
                Where(x => x);

            return selectedGraphs;
        }

        private static void OnAssemblyReloaded()
        {
            if (AssetEditorTools.IsPureEditorMode == false)
                return;
            GetActiveWindows.ForEach(x => x.Reload());
        }

                
        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            GameLog.Log($"PlayMode Changed To: {state}",Color.blue);
            switch (state) {
                case PlayModeStateChange.EnteredEditMode:
                    Reload();
                    break;
            }
        }

        private static void OnBeforeAssemblyReload()
        {
            if (AssetEditorTools.IsPureEditorMode == false)
                return;
            Save();
        }

        
        private static void OnSceneSaved(Scene scene) => Save();
        
    }
}