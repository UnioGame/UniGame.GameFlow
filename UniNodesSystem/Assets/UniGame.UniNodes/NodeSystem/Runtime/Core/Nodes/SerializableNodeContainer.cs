using UnityEngine;

namespace UniGame.UniNodes.NodeSystem.Runtime.Core.Nodes
{
    public class SerializableNodeContainer : ScriptableObject
    {

        public string Type;

        public string FullType;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HideLabel]
        [Sirenix.OdinInspector.InlineProperty]
#endif
        public SerializableNode Node;


        public void Initialize(SerializableNode node)
        {
            Node = node;
            Type = node?.GetType().Name;
            FullType = node?.GetType().AssemblyQualifiedName;
        }
    }
}
