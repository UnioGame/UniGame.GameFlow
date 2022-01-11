namespace UniModules.UniGameFlow.Nodes.Runtime.Scene
{
    using global::UniGame.UniNodes.Nodes.Runtime.Common;
    using global::UniModules.GameFlow.Runtime.Core;
    using NodeSystem.Runtime.Core.Attributes;
    using UnityEngine;

    [CreateNodeMenu("Common/Scene/SceneReference")]
    public class SceneReferenceNode<T> : ContextNode where T : Object
    {
        [SerializeField]
        private T target;
        protected T Target => target;
    }
}
