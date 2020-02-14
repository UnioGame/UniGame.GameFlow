namespace UniGreenModules.UniNodeSystem.Inspector.Editor.Drawers
{
    using System.Collections.Generic;
    using BaseEditor;
    using BaseEditor.Interfaces;
    using Interfaces;
    using Runtime.Core;
    using UniCore.Runtime.ProfilerTools;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Attributes;
    using UnityEditor;
    using UnityEngine;

    public class BaseBodyDrawer : INodeEditorHandler
    {
        private List<string> _excludes;
    
        public BaseBodyDrawer()
        {
            //TODO remove this old dirty hack
            _excludes = new List<string>(){"m_Script", "graph", "position", "ports"};
        }
    
        public bool Update(INodeEditorData editor, Node node)
        {
            var serializedObject = editor.SerializedObject;

            var iterator = serializedObject.GetIterator();
            var enterChildren = true;
            var type = node.GetType();
            
            EditorGUIUtility.labelWidth = 84;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                
                //is node field should be draw
                var field = type.GetField(iterator.name);
                var hideInspector = field?.GetCustomAttributes(typeof(HideNodeInspectorAttribute), false).Length > 0;
                if (hideInspector || _excludes.Contains(iterator.name)) continue;
                
                node.DrawNodePropertyField(iterator,new GUIContent(iterator.name,iterator.tooltip),true);
            }

            return true;
        }
    
    }
}
