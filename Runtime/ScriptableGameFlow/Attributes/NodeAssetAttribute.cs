using System;

namespace UniModules.UniGame.GameFlow.GameFlowEditor.Editor.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class NodeAssetAttribute : Attribute
    {
        public readonly Type NodeType;
        
        public NodeAssetAttribute(Type nodeType)
        {
            NodeType = nodeType;
        }

    }

}
