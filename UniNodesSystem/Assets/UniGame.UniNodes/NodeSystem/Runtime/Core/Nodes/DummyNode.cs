using System;
using System.Collections.Generic;
using UniGame.UniNodes.NodeSystem.Runtime.Core;
using UniGame.UniNodes.NodeSystem.Runtime.Core.Interfaces;
using UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
using UnityEngine;

namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.Nodes
{
    [Serializable]
    public class DummyNode : INode
    {
        public int    Id       { get; }
        public string ItemName { get; }

        public IGraphData              GraphData { get; }
        public IReadOnlyList<NodePort> Ports     { get; }
        public IEnumerable<NodePort>   Outputs   { get; }
    
        public IEnumerable<NodePort> Inputs { get; }

        public NodePort GetOutputPort(string fieldName) => null;

        public NodePort GetInputPort(string fieldName) => null;

        public NodePort GetPort(string fieldName) => null;

        public bool HasPort(string fieldName) => false;

        public Vector2 Position { get; set; } = Vector2.zero;
        public int Width => 220;

        public void OnIdUpdate(int oldId, int newId, IGraphItem updatedItem) { }
        
        public void SetUpData(IGraphData data) { }

        public void SetName(string nodeName) { }

        public void RemovePort(string fieldName) { }

        public void RemovePort(NodePort port) { }

        public void ClearConnections() { }
        public void Initialize(IGraphData data) {}

        public void SetPosition(Vector2 position) { }
        
        public void SetWidth(int nodeWidth) {}

        public NodePort AddPort(string fieldName,
            IReadOnlyList<Type> types, PortIO direction,
            ConnectionType connectionType = ConnectionType.Multiple,
            ShowBackingValue showBackingValue = ShowBackingValue.Always) => null;

    }
}
