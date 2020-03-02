namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor
{
    using System;
    using System.Collections.Generic;
    using Interfaces;
    using Runtime.Core;
    using Runtime.Core.Nodes;
    using Runtime.Interfaces;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    /// <summary> Handles caching of custom editor classes and their target types. Accessible with GetEditor(Type type) </summary>
    public class NodeEditorBase<T, A, K> 
        where A : Attribute, INodeEditorAttribute
        where T : NodeEditorBase<T, A, K>
        where K : class,INode
    {
        #region static data

        /// <summary> Custom editors defined with [CustomNodeEditor] </summary>
        private static Dictionary<Type, Type> _editorsTypesMap;

        private static Dictionary<object, T> editors = new Dictionary<object, T>();

        private static Dictionary<Type, Type> editorTypes
        {
            get
            {
                if (_editorsTypesMap == null)
                {
                    CacheCustomEditors();
                }

                return _editorsTypesMap;
            }
            set => _editorsTypesMap = value;
        }

        public static T GetEditor(INode node)
        {
            if (node == null) return null;
            if (editors.TryGetValue(node, out var editorBase))
                return editorBase;
            return null;
        }

        public static T GetEditor(EditorNode target)
        {
            if (!(target.Node is K node)) return null;

            if (!editors.TryGetValue(node, out var editor))
            {
                var type = node.GetType();
                var editorType = GetEditorType(type);
                
                editor = Activator.CreateInstance(editorType) as T;
                editors.Add(node, editor);
                editor.OnEnable();
            }

            if (editor.Node == null ) {
                editor.Initialize(target,node);
            }

            return editor;
        }

        public static Object CreateTargetAsset(INode targetItem)
        {
            var assetTarget = targetItem as Object;
            if (assetTarget == null)
            {
                return null;
                var assetItem = ScriptableObject.
                    CreateInstance(typeof(SerializableNodeContainer)) as SerializableNodeContainer;
                assetItem.Node = targetItem as SerializableNode;
                assetTarget      = assetItem;
            }

            return assetTarget;
        }


        #endregion
        
        public K Node;
        public EditorNode EditorData;
        
        public SerializedObject SerializedObject { get; protected set; }
        
        public void Initialize(EditorNode editorNode, K node)
        {
            Node = node;
            EditorData = editorNode;
            if(node is Object target)
                SerializedObject = new SerializedObject(target);
        }
        
        public virtual void OnEnable()
        {
            Node = Node ?? SerializedObject?.targetObject as K;
        }
        
        private static Type GetEditorType(Type type)
        {
            if (type == null) return null;
            if (editorTypes == null) CacheCustomEditors();
            Type result;
            if (editorTypes.TryGetValue(type, out result)) return result;
            //If type isn't found, try base type
            return GetEditorType(type.BaseType);
        }

        private static void CacheCustomEditors()
        {
            editorTypes = new Dictionary<Type, Type>();

            //Get all classes deriving from NodeEditor via reflection
            var nodeEditors = NodeEditorWindow.GetDerivedTypes(typeof(T));
            for (var i = 0; i < nodeEditors.Count; i++)
            {
                if (nodeEditors[i].IsAbstract) continue;
                var attribs = nodeEditors[i].GetCustomAttributes(typeof(A), false);
                if (attribs == null || attribs.Length == 0) continue;
                var attrib = attribs[0] as A;
                editorTypes.Add(attrib.GetInspectedType(), nodeEditors[i]);
            }
        }
    }
}