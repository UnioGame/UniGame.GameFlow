namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.Drawers
{
    using System.Collections.Generic;
    using BaseEditor;
    using BaseEditor.Interfaces;
    using Interfaces;
    using Runtime.Attributes;
    using Runtime.Core;
    using UniGreenModules.UniCore.Runtime.ReflectionUtils;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Profiling;

    public class BaseBodyDrawer : INodeEditorHandler
    {
        private List<string> _excludes;
    
        public BaseBodyDrawer()
        {
            //TODO remove this old dirty hack
            _excludes = new List<string>(){"m_Script", "position", "ports", "id"};
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
                var field = type.GetFieldInfo(iterator.name);
                var hideInspector = field?.GetCustomAttributes(typeof(HideNodeInspectorAttribute), false).Length > 0;
   
                if (hideInspector || _excludes.Contains(iterator.name)) continue;
                
                node.DrawNodePropertyField(iterator,new GUIContent(iterator.name,iterator.tooltip),true);

            }

            return true;
        }
    
    }
}
