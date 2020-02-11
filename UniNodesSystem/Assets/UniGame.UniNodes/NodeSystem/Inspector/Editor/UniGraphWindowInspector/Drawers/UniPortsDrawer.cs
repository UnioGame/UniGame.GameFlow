namespace UniGreenModules.UniNodeSystem.Inspector.Editor.Drawers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using BaseEditor.Interfaces;
    using Interfaces;
    using Runtime;
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
        
        public bool Update(INodeEditorData editor, UniBaseNode baseNode)
        {
            _drawedPorts.Clear();
            
            DrawPorts(editor, _drawedPorts);

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

        private void DrawPorts(INodeEditorData editor,IDictionary<string, NodePort> cache)
        {
            var node = editor.Target;
            
            for (var i = 0; i < node.Ports.Count; i++)
            {
                var portValue = node.Ports[i];
                
                if(editor.HandledPorts.ContainsKey(portValue))
                    continue;
                
                var outputPortName = portValue.ItemName;
                
                if (cache.ContainsKey(outputPortName))
                    continue;
                
                var portName = bracketsExpr.Replace(outputPortName, string.Empty, 1);
                var inputPortName = portName.GetFormatedPortName(PortIO.Input);

                //Try Draw port pairs
                var result = DrawPortPair(node, inputPortName, outputPortName);
                
                var portOutput = node.GetPort(outputPortName);
                cache[portName] = portOutput;
                
                if (result)
                {
                    var portInput = node.GetPort(inputPortName);
                    cache[inputPortName] = portInput;
                }
                else
                {
                    DrawPort(portOutput);
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
