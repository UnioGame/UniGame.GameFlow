using GraphProcessor;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endif
using UnityEditor;
using UnityEngine.UIElements;

namespace UniModules.UniGame.GameFlow.GameFlowEditor.Editor.ScriptableGameFlow.Inspector
{
    [CustomEditor(typeof(GameFlowGraph), true)]
    public class ScriptableGameFlowEditor : GraphInspector
    {
#if ODIN_INSPECTOR
        private PropertyTree _propertyTree;
#endif
        
        protected override void OnEnable()
        {
            base.OnEnable();
#if ODIN_INSPECTOR
            _propertyTree = PropertyTree.Create(target);
#endif
        }

        protected override void CreateInspector()
        {
#if ODIN_INSPECTOR
            var imGuiContainer = new IMGUIContainer(() => _propertyTree.Draw());
            root.Add(imGuiContainer);
            return;
#endif
            base.CreateInspector();
            
            root.Add(new Button(() => ScriptableGameFlowWindow.ShowWindow(target as GameFlowGraph))
            {
            	text = "Open Game Flow"
            });
        }
    }
}
