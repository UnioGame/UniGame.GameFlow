namespace UniModules.UniGameFlow.NodeSystem.Runtime.Extensions
{
    using System.Diagnostics;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
    using UniRx;
    using UnityEngine;

    public static class UniGraphEvent
    {
        public readonly static Subject<INode> NodeUpdateStream = new Subject<INode>();

    }
}
