using System;
using GraphProcessor;
using UniCore.Runtime.ProfilerTools;
using UniModules.UniCore.EditorTools.Editor.Utility;
using UniModules.UniCore.Runtime.DataFlow;
using UniModules.UniGame.Core.EditorTools.Editor.AssetOperations;
using UniModules.UniGame.Core.EditorTools.Editor.EditorResources;
using UniModules.UniGame.GameFlow.ScriptableGameFlow.Editor.Views;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniModules.UniGame.GameFlow.ScriptableGameFlow.Editor
{
    public class ScriptableGameFlowWindow : BaseGraphWindow
    {
        #region static data

        private static string defaultTitle = "(null)";

        #endregion

        #region static methods

        public static ScriptableGameFlowWindow Open(GameFlowGraph graph)
        {
            var window = CreateWindow(graph);
            window.Show();
            return window;
        }

        public static ScriptableGameFlowWindow CreateWindow(GameFlowGraph graph)
        {
            var window = SelectWindow(graph);
            window.InitializeGraph(graph);
            return window;
        }

        public static ScriptableGameFlowWindow SelectWindow(GameFlowGraph graph)
        {
            var window = CreateWindow<ScriptableGameFlowWindow>(); 
            return window;
        }

        #endregion

        #region private fields

        private LifeTimeDefinition _lifeTime = new LifeTimeDefinition();
        private Vector2 _minimapPosition = new Vector2(50, 50);
        private Vector2 _settingsPinnedPosition = new Vector2(50, 250);

        private MiniMapView _miniMapView;
        private EditorResource _graphResource;
        private int _graphId;

        #endregion

        public ScriptableGameFlowView AssetGraph { get; protected set; }

        #region public methods

        public void Save()
        {
            if (!AssetEditorTools.IsPureEditorMode)
                return;
            //TODO
        }

        #endregion


        protected override void OnEnable()
        {
            _lifeTime?.Release();

            GameLog.Log("GameFlowWindow : OnEnable");
            graph = null;

            base.OnEnable();
        }

        protected override void OnDestroy()
        {
            _lifeTime.Release();
            base.OnDestroy();
        }

        protected override void InitializeWindow(BaseGraph inputGraph)
        {
            AssetGraph = new ScriptableGameFlowView(this);
            rootView.Add(AssetGraph);
        }

        protected override void InitializeGraphView(BaseGraphView view)
        {
            CreateMinimap(view);
            CreatePinned(view);
        }

        private void CreatePinned(BaseGraphView view)
        {

        }

        private void CreateMinimap(BaseGraphView view)
        {
            _miniMapView = CreateControlView(() => new MiniMapView(view), _miniMapView);
            _miniMapView.SetPosition(new Rect(_minimapPosition, _miniMapView.GetPosition().size));

            view.Add(_miniMapView);
        }

        private TView CreateControlView<TView>(Func<TView> factory, GraphElement view)
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
