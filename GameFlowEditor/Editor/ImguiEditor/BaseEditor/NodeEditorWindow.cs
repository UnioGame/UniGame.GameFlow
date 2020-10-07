namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Runtime.Core;
    using Runtime.Interfaces;
    using UniModules.UniCore.EditorTools.Editor.PrefabTools;
    using UniModules.UniCore.EditorTools.Editor.Utility;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGameFlow.GameFlowEditor.Editor.Tools;
    using UniRx;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [InitializeOnLoad]
    public partial class NodeEditorWindow : EditorWindow
    {
        public const string ActiveGraphPath     = "ActiveGraphPath";
        public const string UniNodesWindowTitle = "UniNodes";

        public static HashSet<NodeEditorWindow> ActiveWindows { get; protected set; } = new HashSet<NodeEditorWindow>();

        private Dictionary<INodePort, Rect>     _portConnectionPoints = new Dictionary<INodePort, Rect>();
        private Dictionary<INode, Vector2> _nodeSizes            = new Dictionary<INode, Vector2>();

        
        private LifeTimeDefinition _lifeTime = new LifeTimeDefinition();
        
        private float   _zoom = 1;
        private Vector2 _panOffset;
        private IDisposable graphUpdateDisposable;

        [SerializeField] private NodePortReference[] _references = new NodePortReference[0];
        [SerializeField] private Rect[]              _rects      = new Rect[0];

        public NodeGraph LastEditorGraph;
        public NodeGraph ActiveGraph;

        private SerializedObject activeObject;
        public SerializedObject ActiveObject {
            get {
                if(ActiveGraph!=null && (activeObject == null || activeObject.targetObject != ActiveGraph))
                    activeObject = new SerializedObject(ActiveGraph);
                return activeObject;
            }
        }
        
        /// <summary> Stores node positions for all nodePorts. </summary>
        public Dictionary<INodePort, Rect> PortConnectionPoints => _portConnectionPoints;

        public Dictionary<INode, Vector2> NodeSizes => _nodeSizes;

        public string Title { get; protected set; }

        public Vector2 PanOffset {
            get => _panOffset;
            set {
                _panOffset = value;
                Repaint();
            }
        }

        public float Zoom {
            get => _zoom;
            set {
                _zoom = Mathf.Clamp(value, 1f, 5f);
                Repaint();
            }
        }

        #region public static methods

        [OnOpenAsset(0)]
        public static bool OnOpen(int instanceID, int line)
        {
            var nodeGraph = EditorUtility.InstanceIDToObject(instanceID) as NodeGraph;
            return nodeGraph != null && Open(nodeGraph);
        }

        public static NodeEditorWindow Open(NodeGraph nodeGraph, bool replaceActive = false)
        {
            if (!nodeGraph) return null;

            var window = SelectWindow(nodeGraph, replaceActive);

            window.Initialize(nodeGraph);
            window.Show();
            return window;
        }

        private static NodeEditorWindow SelectWindow(NodeGraph nodeGraph, bool replaceActive)
        {
            NodeEditorWindow window = null;
            
            if (replaceActive) {
                window = ActiveWindows.FirstOrDefault(x => x != null);
            }

            window = window ?? ActiveWindows.FirstOrDefault(x => x.Title == nodeGraph.name);
            window = window ?? ActiveWindows.FirstOrDefault(x => !x.ActiveGraph);
            window = window ?? CreateInstance<NodeEditorWindow>();

            return window;
        }

        /// <summary> Repaint all open NodeEditorWindows. </summary>
        public static void RepaintAll()
        {
            var windows = Resources.FindObjectsOfTypeAll<NodeEditorWindow>();
            for (var i = 0; i < windows.Length; i++) {
                windows[i].Repaint();
            }
        }

        /// <summary> Create editor window </summary>
        public static NodeEditorWindow Init()
        {
            var w = CreateInstance<NodeEditorWindow>();
            w.titleContent   = new GUIContent("NodeGraph");
            w.wantsMouseMove = true;
            w.Show();
            return w;
        }

        #endregion

        public void Initialize(NodeGraph nodeGraph)
        {
            PortConnectionPoints.Clear();
            titleContent   = new GUIContent(nodeGraph.name);
            Title          = nodeGraph.name;
            wantsMouseMove = true;
            ActiveGraph    = nodeGraph;
            
            if(Application.isPlaying == false)
                nodeGraph.Initialize(nodeGraph);
        }

        public void Save()
        {
            Save(ActiveGraph);
        }

        public void Save(GameObject target, Component component)
        {
            target.gameObject.ApplyComponent(component);
        }

        public void PingInEditor()
        {
            ActiveGraph.PingInEditor();
        }

        public void Refresh()
        {
            if (ActiveGraph == null) return;
            ActiveGraph.Release();
            ActiveGraph.Initialize(ActiveGraph);
        }

        public void OnInspectorUpdate()
        {
            if (!Application.isPlaying)
                return;
            Repaint();
        }

        public void SaveAs()
        {
            var path = EditorUtility.SaveFilePanelInProject("Save NodeGraph", "NewNodeGraph", "asset", "");
            if (string.IsNullOrEmpty(path)) return;
            var existingGraph = AssetDatabase.LoadAssetAtPath<NodeGraph>(path);
            if (existingGraph != null) AssetDatabase.DeleteAsset(path);
            AssetDatabase.CreateAsset(ActiveGraph, path);
            EditorUtility.SetDirty(ActiveGraph);
            if (this.GetSettings().autoSave) AssetDatabase.SaveAssets();
        }

        public Vector2 WindowToGridPosition(Vector2 windowPosition)
        {
            return (windowPosition - (position.size * 0.5f) - (PanOffset / Zoom)) * Zoom;
        }

        public Vector2 GridToWindowPosition(Vector2 gridPosition)
        {
            return (position.size * 0.5f) + (PanOffset / Zoom) + (gridPosition / Zoom);
        }

        public Rect GridToWindowRectNoClipped(Rect gridRect)
        {
            gridRect.position = GridToWindowPositionNoClipped(gridRect.position);
            return gridRect;
        }

        public Rect GridToWindowRect(Rect gridRect)
        {
            gridRect.position =  GridToWindowPosition(gridRect.position);
            gridRect.size     /= Zoom;
            return gridRect;
        }

        public Vector2 GridToWindowPositionNoClipped(Vector2 gridPosition)
        {
            var center  = position.size * 0.5f;
            var xOffset = (center.x * Zoom + (PanOffset.x + gridPosition.x));
            var yOffset = (center.y * Zoom + (PanOffset.y + gridPosition.y));
            return new Vector2(xOffset, yOffset);
        }

        private static NodeGraph GetGraphItem(string assetPath)
        {
            //var loadedGraphObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            var loadedGraphObject = PrefabUtility.LoadPrefabContents(assetPath);
            var targetGraph       = loadedGraphObject.GetComponent<NodeGraph>();
            return targetGraph;
        }

        private void DraggableWindow(int windowID)
        {
            GUI.DragWindow();
        }


        private List<Object> selectionAssets = new List<Object>();

        private void OnSelectionChange()
        {
            selectionAssets.Clear();
            selectionAssets.AddRange(Selection.objects);
            selectionAssets.AddRange(Selection.gameObjects);

            var selections = selectionAssets.OfType<GameObject>();

            NodeGraph target = null;
            foreach (var selection in selections) {
                target = selection.GetComponent<NodeGraph>();
                if (target != null) break;
            }

            if (target == null) return;

            Open(target);
        }

        private void OnFocus()
        {
            graphEditor = NodeGraphEditor.GetEditor(ActiveGraph);
            var settings = this.GetSettings();

            if (graphEditor != null && settings.autoSave) {
                return;
                AssetDatabase.SaveAssets();
            }
        }

        private void OnDisable()
        {
            _lifeTime.Terminate();
        }
        
        private void OnEnable()
        {
            var stateObservable =  Observable.FromEvent(
                    ev => EditorApplication.playModeStateChanged += OnPlayModeChanged, 
                    ev => EditorApplication.playModeStateChanged -= OnPlayModeChanged);
            
            stateObservable.
                Subscribe(x => PortConnectionPoints.Clear()).
                AddTo(_lifeTime);
                    
            stateObservable.
                Subscribe().
                AddTo(_lifeTime);

            Observable.FromEvent(
                    ev => AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload,
                    ev => AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload).
                Subscribe().
                AddTo(_lifeTime);
            
            ActiveWindows.Add(this);
            
            _lifeTime.AddCleanUpAction(() => ActiveWindows.Remove(this));
            _lifeTime.AddCleanUpAction(() => graphUpdateDisposable.Cancel());

            // // Reload portConnectionPoints if there are any
            // var length = _references.Length;
            // if (length == _rects.Length) {
            //     for (var i = 0; i < length; i++) {
            //         var nodePort = _references[i].GetNodePort();
            //         if (nodePort != null) {
            //             _portConnectionPoints[nodePort] = _rects[i];
            //         }
            //     }
            // }
            
            NodeGraph.ActiveGraphs.
                ObserveCountChanged().
                Where(x => ActiveGraph == null).
                Select(x =>  NodeGraph.ActiveGraphs.
                    FirstOrDefault(y => Title == y.name)).
                Do(x => Open(x as UniGraph)).
                Subscribe().
                AddTo(_lifeTime);

            graphEditor?.OnEnable();
        }

        private void OnBeforeAssemblyReload()
        {
            PortConnectionPoints.Clear();
        }

        private void OnPlayModeChanged(PlayModeStateChange modeStateChange)
        {
            switch (modeStateChange) {
                case PlayModeStateChange.EnteredEditMode:
                case PlayModeStateChange.EnteredPlayMode:
                    var activeGraph = EditorGraphTools.FindSceneGraph(Title);
                    Open(activeGraph);
                    break;
            }
        }


        public NodeGraph Save(NodeGraph nodeGraph)
        {
            nodeGraph = nodeGraph.Save();
            activeObject?.ApplyModifiedProperties();
            activeObject = new SerializedObject(nodeGraph);
            return nodeGraph;
        }
    }
}