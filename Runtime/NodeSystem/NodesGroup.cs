namespace UniModules.GameFlow.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class NodesGroup : INodesGroup
    {
        public string    title   = string.Empty;
        public Color     color   = new Color(0, 0, 0, 0.3f);
        public List<int> nodeIds = new List<int>();
        public Rect      position;
        public Vector2   size;

        public Vector2 Size => size;

        public Rect Position => position;

        public List<int> NodeIds => nodeIds;

        public Color Color => color;

        public string Title => title;
    }
}