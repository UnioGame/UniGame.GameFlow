namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using Interfaces;
    using Runtime.Interfaces;

    public interface IPortConnection 
    {
        int PortId { get; }

        int NodeId { get; }

        string PortName { get; }

        INodePort Port { get; }
    }
}