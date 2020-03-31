using GraphProcessor;

namespace UniGame.GameFlowEditor.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Runtime;
    using UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.EditorTools.Editor.AssetOperations;
    using UniNodes.GameFlowEditor.Editor;
    using UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector;
    using UniNodes.NodeSystem.Runtime.Core;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UIElements;

    public class UniGameFlowWindow : BaseGraphWindow
    {
        #region static data

        protected static bool isInitialized;
        protected static HashSet<UniGameFlowWindow> Windows = new HashSet<UniGameFlowWindow>();

        #endregion
        
        #region static methods
 
        public static UniGameFlowWindow Open(UniGraph graph)
        {
            InitializeGlobalEvents();
            
            var window = CreateWindow(graph);
            Windows.Add(window);
            window.Show();
            return window;
        }

        public static UniGameFlowWindow CreateWindow(UniGraph graph)
        {
            var window = Windows.FirstOrDefault(x => x.titleContent.text == graph.name);
            if (window != null && window.ActiveGraph) {
                window.Save();
            }
            
            window = window != null ? window : Windows.FirstOrDefault(x => x.ActiveGraph == null);
            window = window != null ? window : CreateInstance<UniGameFlowWindow>();
            window.Initialize(graph);

            return window;
        }

        public static void InitializeGlobalEvents()
        {
            if (isInitialized)
                return;
            isInitialized = true;
            //Selection.selectionChanged += OnSelectionChange;
        }
        
        
        private static void OnSelectionChange()
        {
            var selections = Selection.objects.
                Concat(Selection.gameObjects).
                OfType<GameObject>();
            
            foreach (var item in selections) {
                var graphData = item.GetComponent<UniGraph>();
                if(!graphData) continue;
                Open(graphData);
            }
        }

        #endregion
        
        #region private fields

        private GameFlowGraphView uniGraphView;
        private UniGraphSettingsPinnedView settingsPinnedView;
        private UniGraphToolbarView graphToolbarView;
        private MiniMapView miniMapView;
        
        #endregion

        public UniGraph ActiveGraph { get; protected set; }

        public UniAssetGraph AssetGraph { get; protected set; }

        #region public methods

        public void Initialize(UniGraph uniGraph)
        {
            ActiveGraph = uniGraph;
            Reload();
        }

        public void Reload()
        {
            if (!ActiveGraph) {
                GameLog.LogError($"{nameof(UniGameFlowWindow)} : Null Source UniGraph data",this);
                return;
            }

            graph = null;
            
            InitializeGraph(ActiveGraph);
            InitializeGraph(CreateAssetGraph(ActiveGraph));
        }
        
        public UniAssetGraph CreateAssetGraph(UniGraph uniGraph)
        {
            ActiveGraph = uniGraph;
            
            AssetGraph = ScriptableObject.CreateInstance<UniAssetGraph>();
            AssetGraph.Activate(ActiveGraph);
            
            return AssetGraph;
        }

        public void Save()
        {
            if (AssetEditorTools.IsPureEditorMode) {
                uniGraphView.Save();
            }
        }
        
        #endregion

        protected override void OnDestroy()
        {
            Windows.Remove(this);
            
            //save graph when ctrl + s pressed
            EditorSceneManager.sceneSaved -= OnSave;
            //redraw editor if assembly reloaded
            AssemblyReloadEvents.afterAssemblyReload  -= OnAssemblyReloaded;
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            EditorApplication.playModeStateChanged    -= OnPlayModeChanged;
            base.OnDestroy();
        }

        private void OnAssemblyReloaded()
        {
            if (AssetEditorTools.IsPureEditorMode == false)
                return;
            Reload();
        }
        
        private void OnBeforeAssemblyReload()
        {
            if (AssetEditorTools.IsPureEditorMode == false)
                return;
            Save();
        }
        
        protected override void InitializeWindow(BaseGraph inputGraph)
        {
            titleContent = new GUIContent(ActiveGraph.name);
            uniGraphView = new GameFlowGraphView(this);
            
            rootView.Add(uniGraphView);
            
            CreateToolbar(uniGraphView);
            BindEvents();
        }
        
        protected override void InitializeGraphView(BaseGraphView view)
        {
            CreateMinimap(view);
            CreatePinned(view);
        }

        protected void InitializeGraph(UniGraph targetGraph)
        {

            var idEditingMode = AssetEditorTools.IsPureEditorMode;
            
            if (idEditingMode == false)
                return;

            foreach (var node in targetGraph.Nodes) {
                node.Initialize(targetGraph);
                node.UpdatePortAttributes();
            }
            
        }

        private void BindEvents()
        {
            //save graph when ctrl + s pressed
            EditorSceneManager.sceneSaved += OnSave;
            //redraw editor if assembly reloaded
            AssemblyReloadEvents.afterAssemblyReload += OnAssemblyReloaded;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private void OnSave(Scene scene) => Save();

        private void OnPlayModeChanged(PlayModeStateChange state)
        {
            switch (state) {
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredEditMode:
                case PlayModeStateChange.EnteredPlayMode:
                    var target = NodeGraph.ActiveGraphs.
                        OfType<UniGraph>().
                        FirstOrDefault(x => x.name == titleContent.text);
                    if(target)
                        Initialize(target);
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }
        
        private void CreateToolbar(BaseGraphView view)
        {
            graphToolbarView = new UniGraphToolbarView(view);
            view.Add(graphToolbarView);
        }

        private void CreatePinned(BaseGraphView view)
        {
            settingsPinnedView = CreateGraphView(() => {
                view.OpenPinned< UniGraphSettingsPinnedView >();
                return view.Q<UniGraphSettingsPinnedView>();
            },settingsPinnedView);
        }
        
        private void CreateMinimap(BaseGraphView view)
        {
            miniMapView = CreateGraphView(() => new MiniMapView(view),miniMapView);
            view.Add(miniMapView);
        }

        private TView CreateGraphView<TView>(Func<TView> factory, GraphElement view)
            where TView : GraphElement
        {
            var hasPosition      = false;
            var settingsPosition = new Rect();
            
            if (view != null) {
                hasPosition      = true;
                settingsPosition = view.GetPosition();
            }

            var factoryView = factory();
            
            if (hasPosition) {
                factoryView.SetPosition(settingsPosition);
            }

            return factoryView;
        }

    }
}
