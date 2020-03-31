namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using Runtime.Interfaces;

    public interface IPortConnectionValidator
    {
        bool Validate(INodePort from, INodePort to);
    }
}