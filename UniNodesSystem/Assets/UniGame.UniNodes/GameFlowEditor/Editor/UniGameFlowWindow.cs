using GraphProcessor;

namespace UniGame.GameFlowEditor.Editor
{
    using Runtime;
    using UniCore.Runtime.ProfilerTools;
    using UniGreenModules.UniCore.EditorTools.Editor.PrefabTools;
    using UniNodes.NodeSystem.Runtime.Core;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;

    public class UniGameFlowWindow : BaseGraphWindow
    {
    
        #region static methods
        
        [MenuItem("UniGame/GameFlow/ShowGraphWindow")]
        public static UniGameFlowWindow Open()
        {
            var graphWindow = GetWindow< UniGameFlowWindow >();
            graphWindow.Show();
            return graphWindow;
        }

        public static UniGameFlowWindow Open(UniGraph graph)
        {
            var graphWindow = GetWindow< UniGameFlowWindow >();
            graphWindow.Initialize(graph);
            graphWindow.Show();
            return graphWindow;
        }

        #endregion
        
        #region private fields

        private GameFlowGraphView uniGraphView;
        
        #endregion

        public UniGraph ActiveGraph { get; protected set; }

        public UniAssetGraph AssetGraph { get; protected set; }

        #region public methods

        public void Initialize(UniGraph uniGraph)
        {
            ActiveGraph = uniGraph;
            Refresh();
        }

        public void Refresh()
        {
            if (!ActiveGraph) {
                GameLog.LogError($"{nameof(UniGameFlowWindow)} : Null Source UniGraph data",this);
                return;
            }
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
            uniGraphView.Save();
        }
        
        #endregion
        
        protected override void InitializeWindow(BaseGraph inputGraph)
        {
            titleContent = new GUIContent(ActiveGraph.name);
            uniGraphView = new GameFlowGraphView(this);
            rootView.Add(uniGraphView);

            BindEvents();
        }

        private void BindEvents()
        {
            //save graph when ctrl + s pressed
            EditorSceneManager.sceneSaved += scene => Save();
            //redraw editor if assembly reloaded
            AssemblyReloadEvents.afterAssemblyReload += Refresh;
            AssemblyReloadEvents.beforeAssemblyReload += Save;
        }
        
    }
}
