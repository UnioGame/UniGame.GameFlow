using GraphProcessor;

namespace UniGame.GameFlowEditor.Editor
{
    using Runtime;
    using UniGreenModules.UniCore.EditorTools.Editor.Utility;
    using UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    [NodeCustomEditor(typeof(UniBaseNode))]
    public class UniNodeView : BaseNodeView
    {
        #region public properties
        
        public UniBaseNode Context { get; protected set; }

        #endregion
        
        public override void Enable()
        {
            Context = nodeTarget as UniBaseNode;
            var sourceNode = Context?.SourceNode;
            
            if(!(sourceNode is Object assetNode))
                return;
            
            var container = new IMGUIContainer(
                () => sourceNode.DrawNode(assetNode)) {
                style = {
                    paddingTop = 4,
                    paddingBottom = 4,
                    marginLeft = 4,
                    marginRight = 4,
                }
            };

            controlsContainer.Add(container);
        }

        
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Open UniNode Script", (e) => OpenUniNodeSourceCode());
            base.BuildContextualMenu(evt);
        }

        private void OpenUniNodeSourceCode()
        {
            Context.SourceNode.
                GetType().
                OpenEditorScript();
        }
        
    }
}
