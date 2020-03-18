namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using Runtime.Interfaces;
    using UnityEngine;

    [Serializable]
    public class PortConnection : IPortConnection
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
     
        [NonSerialized] private IGraphData _data;
        
        [NonSerialized] private INodePort _port;

        public INodePort Port => GetPort();

        public int PortId => portId;

        public int NodeId => nodeId;

        public string PortName => fieldName;
        
        
        
        public PortConnection(int targetPort, int targetNode,string portName)
        {
            portId = targetPort;
            nodeId = targetNode;
            fieldName = portName;
        }
        
        public PortConnection(INodePort connectionPort, int id)
        {
            _port      = connectionPort;
            portId = _port.Id;
            Initialize(_port.Node,id,_port.ItemName);
        }

        public void Initialize(INode data,int id, string portName)
        {
            nodeId = id;
            fieldName = portName;
            Initialize(data.GraphData);
        }
        
        public void Initialize(IGraphData data)
        {
            _data   = data;
        }

        /// <summary>
        /// Returns the port that this <see cref="PortConnection"/> points to
        /// </summary>
        public INodePort GetPort()
        {
            //if (port != null) return port;
            var targetNode = _data.GetNode(nodeId);
            if (targetNode == null)
                return null;
            _port = targetNode.GetPort(fieldName);
            return _port;
        }
    }
}