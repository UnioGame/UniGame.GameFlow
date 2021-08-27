namespace UniModules.GameFlow.Runtime.Core
{
    using Runtime.Interfaces;

    public interface IPortConnectionValidator
    {
        bool Validate(INodePort from, INodePort to);
    }
}