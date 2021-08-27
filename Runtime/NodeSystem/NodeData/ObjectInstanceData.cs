namespace UniModules.GameFlow.Runtime.NodeData
{
    using System;
    using UnityEngine;

    [Serializable]
    public struct ObjectInstanceData
    {
        public Transform Parent;
        public Vector3   Position;
        public bool      StayAtWorld;
        public bool      Immortal;
        public bool      SharedInstance;
    }
}