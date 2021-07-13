using GraphProcessor;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace UniModules.UniGame.GameFlow.GameFlowEditor.Editor.ScriptableGameFlow
{
    public class ScriptableGameFlowWindow : BaseGraphWindow
    {
        #region statis data

        [InitializeOnLoadMethod]
        public static void InitializeFlowWindow()
        {
            MessageBroker.Default.Receive<OpenGameFlowGraphMessage>().Subscribe(x => ShowWindow(x.gameFlowGraph));
        }

        public static void ShowWindow(GameFlowGraph gameFlowGraph)
        {
            var window = GetWindow<ScriptableGameFlowWindow>();
            window.InitializeGraph(gameFlowGraph);
        }

        #endregion
        
        
        protected override void InitializeWindow(BaseGraph flowGraph)
        {
            titleContent = new GUIContent(nameof(ScriptableGameFlowWindow));

            graphView ??= new BaseGraphView(this);

            rootView.Add(graphView);
        }
    }
}
