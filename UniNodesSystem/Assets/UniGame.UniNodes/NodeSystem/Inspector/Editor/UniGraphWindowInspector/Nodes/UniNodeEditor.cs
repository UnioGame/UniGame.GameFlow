namespace UniGreenModules.UniNodeSystem.Inspector.Editor.Nodes
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using BaseEditor;
    using Drawers;
    using Drawers.Interfaces;
    using Runtime.Core;
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

            if (Application.isPlaying == false) {
                    
                UpdatePortAttributes(node);
            
                node.Initialize();

                UpdateData(node);

                VerifyNode(node);

            }
            
            base.OnBodyGUI();

            DrawPorts(node);
            
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        public virtual void UpdateData(UniNode node) { }

        public void VerifyNode(UniNode node)
        {
            node.Ports.RemoveItems(x => IsPortRemoved(node,x), node.RemovePort);
            node.Validate();
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

            foreach (var portField in fields) {
                var data = node.GetPortData(portField, portField.Name);
                if(data.PortData == null)
                    continue;
                
                node.UpdatePortValue(data.PortData);
                
                var value = portField.GetValue(node);

                UpdateSerializedCommands(node,data.PortData, value, portField);
            }

        }

        public void UpdateSerializedCommands(UniNode node,IPortData portData, object value, FieldInfo info)
        {
            if (value == null) return;
            switch (value) {
                case IReactiveSource reactiveSource:

                    var port = node.UpdatePortValue(portData);
                    reactiveSource.Bind(port);
                    
                    break;
            }
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