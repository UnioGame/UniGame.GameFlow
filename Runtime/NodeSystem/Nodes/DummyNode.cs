using UniModules.GameFlow.Runtime.Attributes;
using UniModules.GameFlow.Runtime.Core.Interfaces;
using UniModules.GameFlow.Runtime.Interfaces;
using UniModules.UniGame.Context.Runtime.Connections;
using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;

namespace UniModules.GameFlow.Runtime.Core.Nodes
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Runtime.Attributes;
    using Runtime.Core;
    using Runtime.Core.Interfaces;
    using Runtime.Interfaces;
    using UniModules.UniContextData.Runtime.Entities;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using global::UniGame.Core.Runtime;
    using UniRx;
    using UnityEngine;

    [HideNode]
    [Serializable]
    [CreateNodeMenu("Examples/Draft/DemoNode")]
    public class DummyNode : INode
    {
        public int    Id       { get; private set; }
        public string ItemName { get; private set;  }

        public IContextConnection Context { get; }
        public NodeGraph              GraphData { get; }
        public IEnumerable<INodePort> Ports     { get; }
        public IEnumerable<INodePort>   Outputs   { get; }
    
        public IEnumerable<INodePort> Inputs { get; }

        public INodePort GetOutputPort(string fieldName) => null;

        public INodePort GetInputPort(string fieldName) => null;

        public INodePort GetPort(string fieldName) => null;

        public bool HasPort(string fieldName) => false;

        public bool AddPortValue(INodePort portValue)
        {
            return false;
        }

        public int SetId(int id) => Id = id;

        public Vector2 Position { get; set; } = Vector2.zero;
        
        public int    Width      { get; set; } = 220;
        public string GetStyle() => string.Empty;


        public void OnIdUpdate(int oldId, int newId, IGraphItem updatedItem) { }
        
        public void SetUpData(NodeGraph data) { }

        public void SetName(string nodeName) { }

        public void RemovePort(string fieldName) { }

        public void RemovePort(INodePort port) { }

        public void ClearConnections() { }
        public void Initialize(NodeGraph data) {}

        public NodePort AddPort(string fieldName, IEnumerable<Type> types, PortIO direction,
            ConnectionType connectionType = ConnectionType.Multiple,
            ShowBackingValue showBackingValue = ShowBackingValue.Always, bool distinctValue = false)
        {
            return null;
        }

        public void Validate(){}

        public void SetPosition(Vector2 position) { }
        
        public void SetWidth(int nodeWidth) {}

        public NodePort AddPort(string fieldName,
            IEnumerable<Type> types, 
            PortIO direction,
            ConnectionType connectionType = ConnectionType.Multiple,
            ShowBackingValue showBackingValue = ShowBackingValue.Always) => null;

    }


    [HideNode]
    [CreateNodeMenu("Examples/Draft/DemoNode")]
    public class DemoNode : UniNode
    {
        [Port(PortIO.Input)]
        public object input1;
        [Port(PortIO.Input)]
        public object input2;
        
        [Port(PortIO.Output)]
        public object output;

        protected override UniTask OnExecute()
        {
            var inputValue1 = GetPortValue(nameof(input1));
            var inputValue2 = GetPortValue(nameof(input2));

            var outputValue = GetPortValue(nameof(output));
            
            //Bind Output Port With input data
            //Now All Data from inputs will be transferred to output
            inputValue1.Broadcast(outputValue).AddTo(LifeTime);
            inputValue2.Broadcast(outputValue).AddTo(LifeTime);
            
            return UniTask.CompletedTask;
        }
    }
    
}
