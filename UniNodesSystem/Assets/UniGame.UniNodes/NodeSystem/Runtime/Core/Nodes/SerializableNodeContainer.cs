using UnityEngine;

namespace UniGame.UniNodes.NodeSystem.Runtime.Core.Nodes
{
    public class SerializableNodeContainer<TTarget> : 
        ScriptableObject
    {
        public TTarget target;
    }
}
