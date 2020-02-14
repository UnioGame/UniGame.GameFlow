namespace UniGreenModules.UniNodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class PortConnection
    {
        [SerializeField] public string      fieldName;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineEditor]
#endif
        [SerializeField] public Node node;

        [NonSerialized] private NodePort port;
        
        public                 NodePort Port => port ?? (port = GetPort());


        /// <summary> Extra connection path points for organization </summary>
        [SerializeField]
        public List<Vector2> reroutePoints = new List<Vector2>();

        public PortConnection(NodePort port)
        {
            this.port = port;
            node      = port.Node;
            fieldName = port.ItemName;
        }

        /// <summary> Returns the port that this <see cref="PortConnection"/> points to </summary>
        public NodePort GetPort()
        {
            if (node == null || string.IsNullOrEmpty(fieldName)) return null;
            return node.GetPort(fieldName);
        }
    }
}