namespace UniGame.UniNodes.NodeSystem.Runtime.Interfaces
{
    public interface IUniInOutNode
    {
        IPortValue Input { get; }
        IPortValue Output { get; }
    }
}