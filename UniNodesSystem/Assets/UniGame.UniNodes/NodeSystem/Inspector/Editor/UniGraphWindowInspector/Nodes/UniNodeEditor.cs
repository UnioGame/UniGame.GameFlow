namespace UniGreenModules.UniNodeSystem.Inspector.Editor.Nodes
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using BaseEditor;
    using Drawers;
    using Drawers.Interfaces;
    using Runtime.Extensions;
    using Runtime.Interfaces;
    using Styles;
    using UniCore.EditorTools.Editor.Utility;
    using UniCore.Runtime.Extension;
    using UniCore.Runtime.ProfilerTools;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor.Extensions;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core.Commands;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Core.Interfaces;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Nodes;
    using UnityEngine;

    [CustomNodeEditor(typeof(UniNode))]
    public class UniNodeEditor : NodeEditor, IUniNodeEditor
    {

        #region static data
        
        private static GUIStyle SelectedHeaderStyle = new GUIStyle()
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
        };

        #endregion

        protected List<INodeEditorHandler> bodyDrawers = new List<INodeEditorHandler>();
        
        public override bool IsSelected()
        {
            var node = target as UniNode;
            return node && node.IsActive;
        }

        public override void OnHeaderGUI()
        {
            if (IsSelected())
            {
                EditorDrawerUtils.DrawWithContentColor(Color.red, base.OnHeaderGUI);
                return;
            }
            base.OnHeaderGUI();

        }

        public override void OnBodyGUI()
        {
            var node = target as UniNode;

            UpdatePortAttributes(node);
            
            node.Initialize();

            UpdateData(node);

            base.OnBodyGUI();

            DrawPorts(node);

            node.Ports.RemoveItems(x => IsPortRemoved(node,x), node.RemovePort);
            
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
        
                
        public bool IsPortRemoved(IUniNode node,INodePort port)
        {
            var value = node.PortValues.
                FirstOrDefault(x => x.ItemName == port.ItemName && x.Direction == port.Direction);
            if (value == null) {
                GameLog.Log($"REMOVE PORT {port.FieldName} and Clear");
            }
            
            return value == null;
        }

        public void UpdatePortAttributes(UniNode node)
        {
            var type = node.GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

            //remove all temp commands
            node.nodeSerializableCommands.RemoveAll(x => x.isUpdatable);
            
            foreach (var portField in fields) {
                var data = node.GetPortData(portField, portField.Name);
                if(data == null)
                    continue;
                
                var port = node.UpdatePortValue(data);
                
                var value = portField.GetValue(node);
                if (value is IReactiveSource reactiveSource) {
                    var reactiveSourceCommand = new ReactiveValuePortCommand(port.ItemName, reactiveSource, data , true);
                    node.nodeSerializableCommands.Add(reactiveSourceCommand);
                }

            }

        }

        public virtual void UpdateData(UniNode node)
        {
            
        }

        public void DrawPorts(UniNode node)
        {
            Draw(bodyDrawers);
        }
        

        protected override void OnEditorEnabled()
        {
            base.OnEditorEnabled();
            bodyDrawers = InitializeBodyHandlers(bodyDrawers);
        }
        
        protected virtual List<INodeEditorHandler> InitializeBodyHandlers(List<INodeEditorHandler> drawers)
        {
            drawers.Add(new UniPortsDrawer(new PortStyleSelector()));
            return drawers;
        }


    }
}