namespace UniGreenModules.UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Attributes
{
    using System;
    using UniNodeSystem.Runtime.Core;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class PortValueAttribute : Attribute
    {
        public readonly PortIO Direction;
        public readonly ConnectionType ConnectionType;

        public PortValueAttribute(PortIO direction = PortIO.Input, 
            ConnectionType connectionType = ConnectionType.Multiple)
        {
            this.Direction = direction;
            this.ConnectionType = connectionType;
        }
        
    }
}
