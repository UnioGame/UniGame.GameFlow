namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor
{
    using System;
    using UniModules.GameFlow.Runtime.Core;
    using UniModules.GameFlow.Runtime.Interfaces;
    using UnityEngine;

    public partial class NodeEditorWindow
    {
        [Serializable]
        public class NodePortReference
        {
            [SerializeField] private INode _node;
            [SerializeField] private string _name;

            public NodePortReference(INodePort nodePort)
            {
                _node = nodePort.Node;
                _name = nodePort.ItemName;
            }

            public INodePort GetNodePort()
            {
                if (_node == null)
                {
                    return null;
                }

                return _node.GetPort(_name);
            }
        }
    }
}