namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.BaseEditor
{
    using Runtime.Core;
    using Runtime.Interfaces;
    using UnityEngine;

    public partial class NodeEditorWindow
    {
        [System.Serializable]
        public class NodePortReference
        {
            [SerializeField] private INode _node;
            [SerializeField] private string _name;

            public NodePortReference(NodePort nodePort)
            {
                _node = nodePort.Node;
                _name = nodePort.ItemName;
            }

            public NodePort GetNodePort()
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