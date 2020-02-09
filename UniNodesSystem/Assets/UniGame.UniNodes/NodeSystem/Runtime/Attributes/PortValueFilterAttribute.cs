using System;

namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Attributes
{
    using System.Collections.Generic;
    using System.Linq;
    using UniNodeSystem.Runtime.Core;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class PortValueFilterAttribute : Attribute
    {
        public readonly string portName;
        public readonly PortIO direction;
        public readonly ConnectionType connectionType;
        public readonly List<Type> typeFilter;

        public PortValueFilterAttribute(
            string portName,
            PortIO direction = PortIO.Input, 
            ConnectionType connectionType = ConnectionType.Multiple,
            params Type[] typeFilter)
        {
            this.portName = portName;
            this.direction = direction;
            this.connectionType = connectionType;
            this.typeFilter = typeFilter.ToList();
        }
    
    }
}
