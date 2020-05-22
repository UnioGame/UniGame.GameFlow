namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NodeWidth : Attribute
    {
        public int width;

        /// <summary> Specify a width for this node type </summary>
        /// <param name="width"> Width </param>
        public NodeWidth(int width)
        {
            this.width = width;
        }
    }
}