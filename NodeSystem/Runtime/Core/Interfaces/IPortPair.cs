namespace UniGame.UniNodes.NodeSystem.Runtime.Core.Interfaces
{
    using Runtime.Interfaces;

    public interface IPortPair
    {
        IPortValue InputPort { get; }
        IPortValue OutputPort { get; }
    }
}