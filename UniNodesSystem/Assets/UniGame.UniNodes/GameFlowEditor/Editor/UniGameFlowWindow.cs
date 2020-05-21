using GraphProcessor;

namespace UniGame.GameFlowEditor.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Runtime;
    using UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.EditorTools.Editor.AssetOperations;
    using UniGreenModules.UniCore.EditorTools.Editor.Utility;
    using UniNodes.GameFlowEditor.Editor;
    using UniNodes.NodeSystem.Runtime.Core;
    using UniRx;
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

        private string _graphName = string.Empty;
        private ReactiveProperty<UniGraph> _targetGraph = new ReactiveProperty<UniGraph>();
        private Vector2 _minimapPosition = new Vector2(50,50);
        private Vector2 _settingsPinnedPosition = new Vector2(50,250);
        
        private GameFlowGraphView _uniGraphView;
        private UniGraphSettingsPinnedView _settingsPinnedView;
        private UniGraphToolbarView _graphToolbarView;
        private MiniMapView _miniMapView;
        
        #endregion

        public UniGraph ActiveGraph => _targetGraph.Value;

        public IReadOnlyReactiveProperty<UniGraph> TargetGraph => _targetGraph;

        public UniAssetGraph AssetGraph { get; protected set; }

        #region public methods

        public void Initialize(UniGraph uniGraph)
        {
            _targetGraph.Value = uniGraph;
            Reload();
        }

        public void Reload()
        {
            _targetGraph.Value = ActiveGraph == null ? 
                FindActiveGraph(_graphName) :
                ActiveGraph;
            
            titleContent.text = ActiveGraph == null ? "(null)" : ActiveGraph.name;

            if (!ActiveGraph) {
                GameLog.LogWarning($"{nameof(UniGameFlowWindow)} : Null Source UniGraph data",this);
                return;
            }

            GameLog.Log($"GameFlowWindow : Window Reload [{ActiveGraph.name}]",Color.blue);
            
            LogGraph();
            
            graph = null;

            var assetGraph = CreateAssetGraph(ActiveGraph);

            LogGraph();
            
            InitializeGraph(assetGraph);
            
            LogGraph();
        }

        private void LogGraph()
        {
                        
            var nodes = ActiveGraph.Nodes;
            foreach (var node in nodes) {

                var debug = $"{node.ItemName} : ";
                 
                foreach (var port in node.Ports) {
                    debug += $"{port.ItemName} ";
                }

                GameLog.Log(debug);
            }
        }
        
        public virtual UniAssetGraph CreateAssetGraph(UniGraph uniGraph)
        {
            if (Application.isPlaying == false) {
                uniGraph.Initialize();
                uniGraph.Validate();
            }
            
            _graphName = uniGraph == null ? string.Empty : uniGraph.name;
            
            AssetGraph = ScriptableObject.CreateInstance<UniAssetGraph>();
            AssetGraph.Activate(uniGraph);
            return AssetGraph;
        }

        public void Save()
        {
            if (AssetEditorTools.IsPureEditorMode) {
                _uniGraphView.Save();
            }
        }
        
        #endregion

        protected override void OnEnable()
        {
            GameLog.Log("GameFlowWindow : OnEnable");
            graph = null;
            
            base.OnEnable();
            
            NodeGraph.ActiveGraphs.
                ObserveCountChanged().Subscribe(x => {
                    var target = NodeGraph.ActiveGraphs.
                        FirstOrDefault(y => _graphName == y.name);
                    Initialize(target as UniGraph);
                });
            
            Reload();
        }
        
        protected override void OnDestroy()
        {
            Windows.Remove(this);
            
            //save graph when ctrl + s pressed
            EditorSceneManager.sceneSaved -= OnSave;
            //redraw editor if assembly reloaded
            //AssemblyReloadEvents.afterAssemblyReload  -= OnAssemblyReloaded;
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
            
            graph = null;
            
            Save();
        }
        
        protected override void InitializeWindow(BaseGraph inputGraph)
        {
            titleContent = new GUIContent(ActiveGraph.name);
            _uniGraphView = new GameFlowGraphView(this);
            
            rootView.Add(_uniGraphView);
            
            CreateToolbar(_uniGraphView);
            BindEvents();
        }
        
        protected override void InitializeGraphView(BaseGraphView view)
        {
            CreateMinimap(view);
            CreatePinned(view);
        }

        private void BindEvents()
        {
            //save graph when ctrl + s pressed
            EditorSceneManager.sceneSaved += OnSave;
            //redraw editor if assembly reloaded
            //AssemblyReloadEvents.afterAssemblyReload += OnAssemblyReloaded;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private void OnSave(Scene scene) => Save();

        private void OnPlayModeChanged(PlayModeStateChange state)
        {
            GameLog.Log($"PlayMode Changed To: {state}",Color.blue);
            switch (state) {
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredEditMode:
                case PlayModeStateChange.EnteredPlayMode:
                    Reload();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        private UniGraph FindActiveGraph(string graphName)
        {
            var target = NodeGraph.ActiveGraphs.
                OfType<UniGraph>().
                FirstOrDefault(x => x.name == graphName);
            if (target) return target;
            target = GameObject.FindObjectsOfType<UniGraph>().
                FirstOrDefault(x => x.name == graphName);
            return target;
        }
        
        private void CreateToolbar(BaseGraphView view)
        {
            _graphToolbarView = new UniGraphToolbarView(view);
            view.Add(_graphToolbarView);
        }

        private void CreatePinned(BaseGraphView view)
        {
            _settingsPinnedView = CreateGraphView(() => {
                view.OpenPinned< UniGraphSettingsPinnedView >();
                return view.Q<UniGraphSettingsPinnedView>();
            },_settingsPinnedView);

            AddSettingsCommands(_settingsPinnedView);
            
            _settingsPinnedView.SetPosition(new Rect(_settingsPinnedPosition,_settingsPinnedView.GetPosition().size));

        }

        protected virtual void AddSettingsCommands(IUniGraphSettings settingsView)
        {
            var graphButton = new Button(() => _targetGraph.Value.PingInEditor()) {
                name =  "GraphPingAction",
                text = "Ping",
            };
            settingsView.AddElement(graphButton);
        }
        
        private void CreateMinimap(BaseGraphView view)
        {
            _miniMapView = CreateGraphView(() => new MiniMapView(view),_miniMapView);
            _miniMapView.SetPosition(new Rect(_minimapPosition,_miniMapView.GetPosition().size));
            
            view.Add(_miniMapView);
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
