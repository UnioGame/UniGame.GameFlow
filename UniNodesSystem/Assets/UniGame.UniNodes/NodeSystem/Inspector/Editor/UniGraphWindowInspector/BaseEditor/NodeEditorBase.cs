namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor
{
    using System;
    using System.Collections.Generic;
    using Interfaces;
    using Runtime.Core.Nodes;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    /// <summary> Handles caching of custom editor classes and their target types. Accessible with GetEditor(Type type) </summary>
    public class NodeEditorBase<T, A, K> 
        where A : Attribute, INodeEditorAttribute
        where T : NodeEditorBase<T, A, K>
        where K : class
    {
        #region static data

        /// <summary> Custom editors defined with [CustomNodeEditor] </summary>
        private static Dictionary<Type, Type> _editorsTypesMap;

        private static Dictionary<K, T> editors = new Dictionary<K, T>();

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
            set { _editorsTypesMap = value; }
        }


        public static T GetEditor(K target)
        {
            if (target == null) return null;

            if (!editors.TryGetValue(target, out var editor))
            {
                var type = target.GetType();
                var editorType = GetEditorType(type);
                
                editor = Activator.CreateInstance(editorType) as T;

                var assetTarget = CreateTargetAsset(target);
                editor.target = target;
                editor.serializedObject = new SerializedObject(assetTarget);
                editors.Add(target, editor);
                editor.OnEnable();
            }
            
            if (editor.target == null) 
                editor.target = target;
            if (editor.serializedObject == null) 
                editor.serializedObject = new SerializedObject(CreateTargetAsset(target));

            return editor;
        }

        public static Object CreateTargetAsset(K targetItem)
        {
            var assetTarget = targetItem as Object;
            if (assetTarget == null) {
                var assetItem = ScriptableObject.
                    CreateInstance(typeof(SerializableNodeContainer<K>)) as SerializableNodeContainer<K>;
                assetItem.target = targetItem;
                assetTarget      = assetItem;
            }

            return assetTarget;
        }
        
        #endregion


        public virtual void OnEnable()
        {
            target = serializedObject.targetObject as K;
        }

        public K target;
        public SerializedObject serializedObject;

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