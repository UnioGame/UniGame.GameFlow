namespace UniModules.UniGame.GameFlow.GameFlowEditor.Editor.UiElementsEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.EditorTools.Editor.AssetOperations;
    using global::UniCore.Runtime.ProfilerTools;
    using global::UniGame.GameFlowEditor.Editor;
    using global::UniGame.GameFlowEditor.Runtime;
    using global::UniGame.UniNodes.GameFlowEditor.Editor;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Core;
    using GraphProcessor;
    using UniCore.EditorTools.Editor.Utility;
    using UniCore.Runtime.DataFlow;
    using UniCore.Runtime.Rx.Extensions;
    using UniGameFlow.GameFlowEditor.Editor.Tools;
    using UniRx;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class UniGameFlowWindow : BaseGraphWindow
    {
        #region static data

        protected static bool                       isInitialized;
        protected static HashSet<UniGameFlowWindow> windows   = new HashSet<UniGameFlowWindow>();
        protected static UniGameFlowWindow          focusedWindow;
        
        #endregion

        public static IEnumerable<UniGameFlowWindow> Windows => windows;

        public static UniGameFlowWindow FocusedWindow => focusedWindow;
        
        #region static methods

        public static UniGameFlowWindow Open(UniGraph graph)
        {
            InitializeGlobalEvents();

            var window = CreateWindow(graph);
            window.Show();
            return window;
        }

        public static UniGameFlowWindow CreateWindow(UniGraph graph) {
            var window = SelectWindow(graph);
            window.Initialize(graph);
            return window;
        }

        public static UniGameFlowWindow SelectWindow(UniGraph graph) 
        {
            var window = windows.FirstOrDefault(x => x.titleContent.text == graph.name);
            if (window != null && window.ActiveGraph) {
                window.Save();
            }

            window = window != null ? 
                window : 
                windows.FirstOrDefault(x => x.IsEmpty);
            window = window != null ? 
                window : 
                CreateInstance<UniGameFlowWindow>();

            return window;
        }

        public static void InitializeGlobalEvents()
        {
            if (isInitialized)
                return;
            isInitialized = true;
        }

        #endregion

        #region private fields

        private LifeTimeDefinition         _lifeTime               = new LifeTimeDefinition();
        private string                     _graphName              = string.Empty;
        private ReactiveProperty<UniGraph> _targetGraph            = new ReactiveProperty<UniGraph>();
        private Vector2                    _minimapPosition        = new Vector2(50, 50);
        private Vector2                    _settingsPinnedPosition = new Vector2(50, 250);

        private GameFlowGraphView          _uniGraphView;
        private UniGraphSettingsPinnedView _settingsPinnedView;
        private UniGraphToolbarView        _graphToolbarView;
        private MiniMapView                _miniMapView;

        #endregion

        public bool IsEmpty => !ActiveGraph;
        
        public UniGraph ActiveGraph => _targetGraph.Value;

        public bool IsFocused => FocusedWindow == this;

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
            _targetGraph.Value = GetActiveGraph();

            titleContent.text = ActiveGraph == null ? "(null)" : ActiveGraph.name;

            if (!ActiveGraph) {
                GameLog.LogWarning($"{nameof(UniGameFlowWindow)} : Null Source UniGraph data", this);
                return;
            }

            GameLog.Log($"GameFlowWindow : Window Reload [{ActiveGraph.name}]", Color.blue);

            graph = null;

            var assetGraph = CreateAssetGraph(ActiveGraph);

            InitializeGraph(assetGraph);
        }

        private UniGraph GetActiveGraph()
        {
            var graphName = string.IsNullOrEmpty(_graphName) ? 
                titleContent.text : _graphName;

            var currentGraph = !ActiveGraph ? 
                EditorGraphTools.FindSceneGraph(graphName) : 
                ActiveGraph;
            
            if (!currentGraph) {
                currentGraph = NodeGraph.ActiveGraphs.
                    OfType<UniGraph>().
                    FirstOrDefault(x => x.ItemName == graphName);
            }

            return currentGraph;
        }

        public virtual UniAssetGraph CreateAssetGraph(UniGraph uniGraph)
        {
            if (Application.isPlaying == false) {
                InitializeGraph(uniGraph);
            }

            var graphAsset = ScriptableObject.CreateInstance<UniAssetGraph>();

            if (AssetGraph && uniGraph.name == _graphName) {
                graphAsset.position = AssetGraph.position;
                graphAsset.scale    = AssetGraph.scale;
            }

            _graphName      = uniGraph == null ? string.Empty : uniGraph.name;
            graphAsset.name = _graphName;
            AssetGraph      = graphAsset;
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

        private void OnFocus() {
            focusedWindow = this;
        }
        
        private void InitializeGraph(UniGraph uniGraph)
        {
            if (!AssetEditorTools.IsPureEditorMode) {
                return;
            }

            uniGraph.Initialize();
            uniGraph.Validate();
        }

        protected override void OnEnable()
        {
            _lifeTime?.Release();
            
            GameLog.Log("GameFlowWindow : OnEnable");
            graph = null;

            base.OnEnable();

            NodeGraph.ActiveGraphs.
                ObserveCountChanged().
                Where(x => !ActiveGraph).
                Do(x => Reload()).
                Subscribe().
                AddTo(_lifeTime);

            _lifeTime.AddCleanUpAction(() =>
                windows.Remove(this));
            
            windows.Add(this);
            
            Reload();
        }

        protected override void OnDestroy()
        {
            _lifeTime.Release();
            base.OnDestroy();
        }

        protected override void InitializeWindow(BaseGraph inputGraph)
        {
            if (!ActiveGraph)
                return;

            titleContent  = new GUIContent(ActiveGraph.name);
            _uniGraphView = new GameFlowGraphView(this);

            rootView.Add(_uniGraphView);

            CreateToolbar(_uniGraphView);
        }

        protected override void InitializeGraphView(BaseGraphView view)
        {
            CreateMinimap(view);
            CreatePinned(view);
        }

        private void CreateToolbar(BaseGraphView view)
        {
            _graphToolbarView = new UniGraphToolbarView(view);
            view.Add(_graphToolbarView);
        }

        private void CreatePinned(BaseGraphView view)
        {
            _settingsPinnedView = CreateGraphView(() => {
                view.OpenPinned<UniGraphSettingsPinnedView>();
                return view.Q<UniGraphSettingsPinnedView>();
            }, _settingsPinnedView);

            AddSettingsCommands(_settingsPinnedView);

            _settingsPinnedView.SetPosition(new Rect(_settingsPinnedPosition, _settingsPinnedView.GetPosition().size));
        }

        protected virtual void AddSettingsCommands(IUniGraphSettings settingsView)
        {
            var graphButton = new Button(() => _targetGraph.Value.PingInEditor()) {
                name = "GraphPingAction",
                text = "Ping",
            };
            settingsView.AddElement(graphButton);
        }

        private void CreateMinimap(BaseGraphView view)
        {
            _miniMapView = CreateGraphView(() => new MiniMapView(view), _miniMapView);
            _miniMapView.SetPosition(new Rect(_minimapPosition, _miniMapView.GetPosition().size));

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