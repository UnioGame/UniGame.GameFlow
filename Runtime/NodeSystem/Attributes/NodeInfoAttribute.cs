namespace UniModules.GameFlow.Runtime.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class NodeInfoAttribute : Attribute, INodeInfo {
        public readonly string name;
        public readonly string category;
        public readonly string description;

        public NodeInfoAttribute(string name = "", string category = "default", string description = "") {
            this.name        = name;
            this.category    = category;
            this.description = description;
        }


        public string Name        => this.name;
        public string Category    => this.category;
        public string Description => this.description;

    }
}