using UnityEngine;

namespace UniGame.UniNodes.NodeSystem.Runtime.Core.Nodes
{
    using UniGreenModules.UniCore.EditorTools.Editor.AssetOperations;
    using UniModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.GameFlowEditor.Editor.Tools;
    using UnityEditor;

    public class SerializableNodeContainer : ScriptableObject
    {
        public MonoScript script;
        [HideInInspector]
        public string type;
        [HideInInspector]
        public string fullType;
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HideLabel]
        [Sirenix.OdinInspector.InlineProperty]
        [Sirenix.OdinInspector.CustomValueDrawer(nameof(DrawNode))]
#endif
        [SerializeReference]
        public SerializableNode node;

        public SerializableNodeContainer Initialize(SerializableNode target)
        {
            node = target;
            var nodeType = node?.GetType();
            type = node?.GetType().Name;
            script = AssetEditorTools.GetScriptAsset(nodeType);
            fullType = node?.GetType().AssemblyQualifiedName;
            return this;
        }

#if ODIN_INSPECTOR

        public SerializableNode DrawNode(SerializableNode node, GUIContent label)
        {
            node.DrawNodeImGui();
            return node;
        }
        
#endif
        
    }
}
