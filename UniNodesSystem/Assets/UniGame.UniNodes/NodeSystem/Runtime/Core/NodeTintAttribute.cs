namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using UnityEngine;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NodeTint : Attribute
    {
        public Color color;

        /// <summary> Specify a color for this node type </summary>
        /// <param name="r"> Red [0.0f .. 1.0f] </param>
        /// <param name="g"> Green [0.0f .. 1.0f] </param>
        /// <param name="b"> Blue [0.0f .. 1.0f] </param>
        public NodeTint(float r, float g, float b)
        {
            color = new Color(r, g, b);
        }

        /// <summary> Specify a color for this node type </summary>
        /// <param name="hex"> HEX color value </param>
        public NodeTint(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out color);
        }

        /// <summary> Specify a color for this node type </summary>
        /// <param name="r"> Red [0 .. 255] </param>
        /// <param name="g"> Green [0 .. 255] </param>
        /// <param name="b"> Blue [0 .. 255] </param>
        public NodeTint(byte r, byte g, byte b)
        {
            color = new Color32(r, g, b, byte.MaxValue);
        }
    }
}