using UniCore.Runtime.ProfilerTools;
using UniGame.GameFlowEditor.Editor;
using UniGame.GameFlowEditor.Runtime;
using UniGame.UniNodes.GameFlowEditor.Editor;
using UniModules.GameFlow.Runtime.Core;
using UniModules.UniCore.Runtime.Rx.Extensions;
using UniModules.UniGame.GameFlow.Editor.UiElementsEditor.Tools.ExposedParameterElement;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;

namespace UniModules.GameFlow.Editor
{
    using System;
    using System.Linq;
    using UniModules.Editor;
    using GraphProcessor;
    using UniCore.Runtime.DataFlow;
    using UniRx;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class UniGameFlowWindow : BaseGraphWindow
    {
        #region static data

        private static string defaultTitle = "(null)";
        protected static bool isInitialized;
        protected static ReactiveCollection<UniGameFlowWindow> windows = new ReactiveCollection<UniGameFlowWindow>();
        protected static UniGameFlowWindow focusedWindow;

        
        public static IReadOnlyReactiveCollection<UniGameFlowWindow> Windows => windows;

        public static UniGameFlowWindow FocusedWindow => focusedWindow;

        public static string DefaultTitle => defaultTitle;

        public static UniGameFlowWindow Open(UniGraph graph)
        {
            var window = CreateWindow(graph);
            window.Show();
            return window;
        }

        public static UniGameFlowWindow CreateWindow(UniGraph graph)
        {
            var window = SelectWindow(graph);
            window.Initialize(graph);
            return window;
        }

        public static UniGameFlowWindow SelectWindow(UniGraph graph)
        {
            var window = windows.FirstOrDefault(x => x.titleContent.text == graph.name && x.IsEmpty);
            window = window ? window : windows.FirstOrDefault(x => x.IsEmpty);
            window = window ? window : CreateInstance<UniGameFlowWindow>();

            return window;
        }
        
        #endregion
        
        #region private fields

        private string _guid = string.Empty;
        private LifeTimeDefinition _lifeTime = new LifeTimeDefinition();
        private string _graphName = string.Empty;
        private UniGraph _targetGraph;
        
        private Vector2 _minimapPosition = new Vector2(50, 50);
        private Vector2 _settingsPinnedPosition = new Vector2(50, 250);
        private Vector2 _parametersPinnedPosition = new Vector2(50, 550);
        private Vector2 _parametersPinnedSize = new Vector2(250, 200);

        private GameFlowGraphView _uniGraphView;
        private UniGraphSettingsPinnedView _settingsPinnedView;
        private UniGraphToolbarView _graphToolbarView;
        private MiniMapView _miniMapView;
        private UniExposedParameterView _exposedParameterView;
        private EditorResource _graphResource;
        private int _graphId;

        #endregion

        public bool IsVisible => rootView.visible;

        public string Guid => _guid;

        public int Id => _graphId;

        public string GraphName => _graphName;

        public bool IsEmpty => !ActiveGraph;

        public UniGraph ActiveGraph => _targetGraph;

        public bool IsActiveGraph => ActiveGraph && ActiveGraph.IsActive;

        public bool IsFocused => FocusedWindow == this;

        public EditorResource Resource => _graphResource;

        public UniGraphAsset AssetGraph { get; protected set; }

        #region public methods

        public void Initialize(UniGraph uniGraph)
        {
            _targetGraph = uniGraph;
            _graphId = uniGraph.id;
            _guid = uniGraph.Guid;

            Reload();
        }

        public void AddNode(Type type, string itemName)
        {
            AddNode(type, itemName, _uniGraphView.LastMenuPosition);
        }

        public void AddNode(Type type, string itemName, Vector2 nodePosition)
        {
            if (IsEmpty) return;
            ActiveGraph.AddNode(type, itemName, nodePosition);
            Reload();
        }

        public void Reload()
        {
            if (!ActiveGraph)
            {
                GameLog.LogWarning($"{nameof(UniGameFlowWindow)} : Null Source UniGraph data", this);
                return;
            }

            GameLog.Log($"GameFlowWindow : Window Reload [{ActiveGraph.name}]", Color.blue);

            graph = null;

            var assetGraph = CreateAssetGraph(ActiveGraph);

            InitializeGraph(assetGraph);
        }

        public virtual UniGraphAsset CreateAssetGraph(UniGraph uniGraph)
        {
            if (Application.isPlaying == false)
                InitializeGraph(uniGraph);

            _graphName = uniGraph.name;
            var graphAsset = uniGraph.useVariants
                ? CreateInstance<UniGraphAsset>()
                : uniGraph.serializedGraph;

            if (!graphAsset && Application.isPlaying == false)
            {
                graphAsset = CreateInstance<UniGraphAsset>();
                uniGraph.serializedGraph = graphAsset;
                graphAsset.SaveAssetAsNested(uniGraph.gameObject);
                uniGraph.gameObject.MarkDirty();
            }

            graphAsset.name = _graphName;

            if (AssetGraph && uniGraph.name == _graphName)
            {
                graphAsset.position = AssetGraph.position;
                graphAsset.scale = AssetGraph.scale;
            }

            var sourceGraph = uniGraph;
            if (!uniGraph.useVariants)
            {
                var stage = PrefabStageUtility.GetPrefabStage(uniGraph.gameObject);
                if (Application.isPlaying == false && stage != null && !string.IsNullOrEmpty(stage.assetPath))
                {
                    var asset = AssetDatabase.LoadAssetAtPath<GameObject>(stage.assetPath);
                    sourceGraph = asset.GetComponent<UniGraph>();
                }
            }
            
            AssetGraph = graphAsset;
            AssetGraph.ConnectToGraph(sourceGraph);

            return AssetGraph;
        }

        public void Save()
        {
            if (AssetEditorTools.IsPureEditorMode)
                _uniGraphView.Save();
        }

        #endregion

        private void OnFocus()
        {
            focusedWindow = this;
        }

        private void InitializeGraph(UniGraph uniGraph)
        {
            if (!AssetEditorTools.IsPureEditorMode)
                return;

            uniGraph.Initialize();
            uniGraph.Validate();
        }

        protected override void OnEnable()
        {
            _lifeTime?.Release();

            GameLog.Log("GameFlowWindow : OnEnable");
            graph = null;

            base.OnEnable();

            _lifeTime.AddCleanUpAction(() => windows.Remove(this));
            
            if (!windows.Contains(this))
                windows.Add(this);

            MessageBroker.Default
                .Receive<UniGraphSaveMessage>()
                .Where(x => x.graph == _targetGraph)
                .Do(x => Save())
                .RxSubscribe()
                .AddTo(_lifeTime);
            
            MessageBroker.Default
                .Receive<UniGraphReloadMessage>()
                .Where(x => x.graph == _targetGraph)
                .Do(x => Save())
                .Do(x => Reload())
                .RxSubscribe()
                .AddTo(_lifeTime);
            
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

            titleContent = new GUIContent(ActiveGraph.name);
            _uniGraphView = new GameFlowGraphView(this);

            rootView.Add(_uniGraphView);

            CreateToolbar(_uniGraphView);
        }

        protected override void InitializeGraphView(BaseGraphView view)
        {
            CreateMinimap(view);
            CreateSettingsPinned(view);
            CreateExposedPinnedParameters(view);
        }

        private void CreateToolbar(BaseGraphView view)
        {
            _graphToolbarView = new UniGraphToolbarView(view);
            _graphToolbarView.AddTogglePinnedViewButton<UniExposedParameterView>("Parameters",true);
            view.Add(_graphToolbarView);
        }

        private void CreateSettingsPinned(BaseGraphView view)
        {
            _settingsPinnedView = CreateGraphView(() =>
            {
                view.OpenPinned<UniGraphSettingsPinnedView>();
                return view.Q<UniGraphSettingsPinnedView>();
            }, _settingsPinnedView);

            AddSettingsCommands(_settingsPinnedView);

            _settingsPinnedView.SetPosition(new Rect(_settingsPinnedPosition, _settingsPinnedView.GetPosition().size));
        }

        protected virtual void AddSettingsCommands(IUniGraphSettings settingsView)
        {
            var graphButton = new Button(() => 
                _targetGraph.PingInEditor()) { name = "GraphPingAction", text = "Ping", };
            
            settingsView.AddElement(graphButton);
        }

        private void CreateMinimap(BaseGraphView view)
        {
            _miniMapView = CreateGraphView(() => new MiniMapView(view), _miniMapView);
            _miniMapView.SetPosition(new Rect(_minimapPosition, _miniMapView.GetPosition().size));

            view.Add(_miniMapView);
        }

        private void CreateExposedPinnedParameters(BaseGraphView view)
        {
            _exposedParameterView = view.Q<UniExposedParameterView>();
            if (_exposedParameterView != null) return;
            
            view.OpenPinned<UniExposedParameterView>();
            _exposedParameterView = view.Q<UniExposedParameterView>();
            _exposedParameterView.SetPosition(new Rect(_parametersPinnedPosition, _parametersPinnedSize));
        }

        private TView CreateGraphView<TView>(Func<TView> factory, GraphElement view)
            where TView : GraphElement
        {
            var hasPosition = false;
            var settingsPosition = new Rect();

            if (view != null)
            {
                hasPosition = true;
                settingsPosition = view.GetPosition();
            }

            var factoryView = factory();

            if (hasPosition)
            {
                factoryView.SetPosition(settingsPosition);
            }

            return factoryView;
        }
    }
}