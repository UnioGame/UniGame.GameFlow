using UnityEngine;

namespace UniGame.UniNodes.NodeSystem.Runtime.Core.Nodes
{
    using System;
    using System.IO;
    using System.Linq;
    using Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.Attributes;
    using Object = UnityEngine.Object;

    public class SerializableNodeContainer : ScriptableObject
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.CustomValueDrawer(nameof(DrawScriptField))]
#endif
        public Object script;
        [HideInInspector]
        public string type;
        [HideInInspector]
        public string fullType;
        
//#if ODIN_INSPECTOR
//        [Sirenix.OdinInspector.HideLabel]
//        [Sirenix.OdinInspector.InlineProperty]
//#endif
        [SerializeReference]
        public SerializableNode Node;

        public SerializableNodeContainer Initialize(SerializableNode node)
        {
            Node = node;
            type = node?.GetType().Name;
            fullType = node?.GetType().AssemblyQualifiedName;
            return this;
        }
        
        private Object DrawScriptField(Object target,GUIContent label)
        {
#if UNITY_EDITOR
            var typeName  = Node.GetType().Name;
            var filter    = $"t:script {typeName} ";
            var assetsGuid = UnityEditor.AssetDatabase.FindAssets(filter);
            var path = string.Empty;
            for (var i = 0; i < assetsGuid.Length; i++) {
                var pathItem = UnityEditor.AssetDatabase.GUIDToAssetPath(assetsGuid[i]);
                var assetPathName = Path.GetFileNameWithoutExtension(pathItem);
                if (string.Equals(assetPathName, typeName, StringComparison.OrdinalIgnoreCase)) {
                    path = pathItem;
                    break;
                }
            }
            
            if (string.IsNullOrEmpty(path))
                return null;
            
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(path);
            if (asset == null)
                return null;
            
            UnityEditor.EditorGUI.BeginDisabledGroup(true);
            UnityEditor.EditorGUILayout.ObjectField(new GUIContent(), asset, null, false);
            UnityEditor.EditorGUI.EndDisabledGroup();

            return asset;
#endif
            return null;
        }
        
    }
}
