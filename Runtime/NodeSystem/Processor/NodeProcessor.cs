namespace UniModules.UniGameFlow.GameFlowEditor.Editor.Processor
{
    using System.Collections.Generic;
    using System.Reflection;
    using Extensions;
    using global::UniModules.GameFlow.Runtime.Extensions;
    using global::UniModules.GameFlow.Runtime.Interfaces;
    using NodeSystem.Runtime.Extensions;
    using UniModules.UniCore.Runtime.ReflectionUtils;
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/GameFlow/NodesEditorHandler",fileName = nameof(NodeProcessor))]
    public class NodeProcessor : ScriptableObject
    {
        
        [SerializeReference]
        public List<INodeHandler> NodeHandlers = new List<INodeHandler>() {
            new BaseNodeHandler()
        };
        
        [SerializeReference]
        public List<IPortHandler> PortHandlers = new List<IPortHandler>() {
            new ReactivePortHandler()
        };

        public IEnumerable<PortField> GetNodePortsData(INode node)
        {
            if (node == null)
                yield break;
            
            var type   = node.GetType();
            var fields = type.GetInstanceFields();
            foreach (var field in fields) {
                var data = UpdatePortField(node, field);
                if(data.PortData == null) continue;
                yield return data;
            }
        }
        
                        
        public void UpdatePorts(INode node)
        {
            if (Application.isPlaying)
                return;
            
            foreach (var data in GetNodePortsData(node)) {
                var portValue = data.PortData;
                if(portValue == null)
                    continue;
                        
                var port  = node.UpdatePort(portValue);
                var value = data.Value;

                UpdatePortValue(node, port, value);
            }

        }

        public PortField UpdatePortField(INode node, FieldInfo fieldInfo)
        {
            foreach (var nodeHandler in NodeHandlers) {
                var data = nodeHandler.UpdatePortFieldData(node, fieldInfo);
                if (data.PortData == null)
                    continue;
                return data;
            }

            return new PortField() {
                FieldInfo = fieldInfo,
            };
        }

        public bool UpdatePortValue(INode node, INodePort port, object fieldValue)
        {
            foreach (var portHandler in PortHandlers) {
                if (portHandler.UpdatePortValue(node, port, fieldValue)) {
                    return true;
                }
            }

            return false;
        }
    }
}