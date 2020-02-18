namespace UniGame.UniNodes.NodeSystem.Inspector.Editor.UniGraphWindowInspector.Drawers
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using BaseEditor.Interfaces;
    using Interfaces;
    using Runtime.Core;
    using Runtime.Extensions;
    using Runtime.Interfaces;
    using Styles;

    public class UniPortsDrawer : INodeEditorHandler
    {
        private readonly IPortStyleProvider styleSelector;
        
        private Regex bracketsExpr = new Regex(UniNodeExtension.InputPattern);
        private Dictionary<string, NodePort> _drawedPorts = new Dictionary<string, NodePort>();

        public UniPortsDrawer(IPortStyleProvider styleProvider)
        {
            this.styleSelector = styleProvider;
        }
        
        public bool Update(INodeEditorData editor, Node baseNode)
        {

            DrawPorts(editor);

            return true;

        }
    
        
        public bool DrawPortPair(
            INode node, 
            string inputPortName, 
            string outputPortName)
        {
            var inputPort = node.GetPort(inputPortName);
            var outputPort = node.GetPort(outputPortName);

            if (inputPort == outputPort)
                return false;
            
            return DrawPortPair(node,inputPort, outputPort);
            
        }

        public bool DrawPortPair(INode node,NodePort inputPort, NodePort outputPort)
        {
            if (outputPort == null || inputPort == null)
            {
                return false;
            }

            var inputStyle = styleSelector.Select(inputPort);
            var outputStyle = styleSelector.Select(outputPort);

            node.DrawPortPairField(inputPort,outputPort, inputStyle, outputStyle);
            
            return true;
        }

        private void DrawPorts(INodeEditorData editor)
        {
            _drawedPorts.Clear();

            var node = editor.Target;
            
            for (var i = 0; i < node.Ports.Count; i++)
            {
                var portValue = node.Ports[i];
                
                if(editor.HandledPorts.ContainsKey(portValue))
                    continue;
                
                if (_drawedPorts.ContainsKey(portValue.ItemName) )
                    continue;
                
                var portName = bracketsExpr.Replace(portValue.ItemName, string.Empty, 1);
                var direction = portValue.direction;
                var outputPortName = portName.GetFormatedPortName(PortIO.Output);
                var inputPortName = portName.GetFormatedPortName(PortIO.Input);

                //Try Draw port pairs
                var result = DrawPortPair(node, inputPortName, outputPortName);
                
                if (result)
                {
                    var portInput = node.GetPort(inputPortName);
                    var portOutput = node.GetPort(outputPortName);
                    _drawedPorts[inputPortName] = portInput;
                    _drawedPorts[outputPortName] = portOutput;
                }
                else
                {
                    DrawPort(portValue);
                    _drawedPorts[portValue.ItemName] = portValue;
                }
            }
        }

        public void DrawPort(NodePort port)
        {
            var portStyle = styleSelector.Select(port);
            port.DrawPortField(portStyle);
        }

    }
}
