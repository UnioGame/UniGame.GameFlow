namespace UniModules.GameFlow.Runtime.Core
{
    using System.Collections.Generic;
    using UnityEngine;

    public interface INodesGroup
    {
        Vector2 Size { get; }
        Rect Position { get; }
        List<int> NodeIds { get; }
        Color Color { get; }
        string Title { get; }
    }
}