namespace UniGame.UniNodes.NodeSystem.Runtime.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class NodeInfoAttribute : Attribute
    {
        public string Category;
        public string Name;
        public string Description;
    }
}