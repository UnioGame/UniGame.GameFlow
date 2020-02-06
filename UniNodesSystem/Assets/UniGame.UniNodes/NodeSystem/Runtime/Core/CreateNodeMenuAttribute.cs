namespace UniGreenModules.UniNodeSystem.Runtime.Core
{
    using System;
    using System.IO;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CreateNodeMenuAttribute : Attribute
    {
        public string menuName;
        public string nodeName;

        /// <summary> Manually supply node class with a context menu path </summary>
        /// <param name="menuName"> Path to this node in the context menu. Null or empty hides it. </param>
        public CreateNodeMenuAttribute(string menuName,string nodeName = null)
        {
            this.menuName = menuName;
            this.nodeName = string.IsNullOrEmpty(nodeName) ? Path.GetFileName(menuName) : nodeName;
        }
    }
}