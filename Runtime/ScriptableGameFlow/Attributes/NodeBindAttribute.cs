using System;

namespace UniModules.UniGame.GameFlow.GameFlowEditor.Editor.Attributes
{
    using global::UniGame.GameFlowEditor.Runtime;
    using global::UniModules.GameFlow.Runtime.Interfaces;
    using UnityEngine;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class NodeBindAttribute : Attribute
    {
        private static Type dataType     = typeof(UniBaseNode);
        private static Type baseNodeType = typeof(INode);
        
        public readonly Type NodeData;
        public readonly Type NodeType;
        
        public NodeBindAttribute(Type nodeData,Type nodeType)
        {
            NodeData = nodeData;
            NodeType     = nodeType;
            Validate();
        }

        public void Validate()
        {
            if (NodeData == null || NodeType == null)
            {
                Debug.LogError($"Empty node bind Data {NodeData} Node {NodeType}");
                return;
            }

            if (!dataType.IsAssignableFrom(NodeData) || !baseNodeType.IsAssignableFrom(NodeType))
            {
                Debug.LogError($"Type validation failed");
                return;
            }
            
        }
        
    }

}
