namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using Runtime.Interfaces;
    using UnityEngine;

    [Serializable]
    public class PortConnection
    {
        #region inspector

        [SerializeField] public string fieldName;

        [SerializeField] public int nodeId;

        /// <summary>
        /// Extra connection path points for organization
        /// </summary>
        [SerializeField] public List<Vector2> reroutePoints = new List<Vector2>();

        [SerializeField] public int portId;
    
        #endregion
     
        [NonSerialized] private INode node;
        
        [NonSerialized] private NodePort port;

        public NodePort Port => GetPort();

        public PortConnection(NodePort connectionPort, int id)
        {
            port      = connectionPort;
            portId = port.id;
            
            Initialize(port.node,id,port.fieldName);
        }

        public void Initialize(INode data,int id, string portName)
        {
            nodeId = id;
            fieldName = portName;
            Initialize(data);
        }
        
        public void Initialize(INode data)
        {
            node   = data;
        }

        /// <summary>
        /// Returns the port that this <see cref="PortConnection"/> points to
        /// </summary>
        public NodePort GetPort()
        {
            //if (port != null) return port;
            var targetNode = node.GraphData.GetNode(nodeId);
            if (targetNode == null)
                return null;
            port = targetNode.GetPort(fieldName);
            return port;
        }
    }
}